using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarcusRent.Classes;
using MarcusRent.Data;
using MarcusRent.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MarcusRent.Repositories;
using MarcusRental2.Repositories;
using System.Data;

namespace MarcusRent.Controllers
{
    public class OrderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly ICarRepository _carRepository;
        public OrderController(UserManager<ApplicationUser> userManager, IMapper mapper, IOrderRepository orderRepository, ICarRepository carRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _carRepository = carRepository;
        }

        //GET: Order
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["TempData"] = "Endast inloggade kunder kan se sina bokningar.";
                //return RedirectToAction("Login", "Account");
                //return RedirectToAction("Login", "Account", new { area = "Identity" });
                return Redirect("/Identity/Account/Login");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var isAdmin = userRoles.Contains("Admin");

            IEnumerable<Order> orders;

            if (isAdmin)
            {

                orders = await _orderRepository.GetAllOrdersAsync();
            }
            else
            {
                orders = await _orderRepository.GetOrdersByUserIdAsync(user.Id);
            }

            var model = _mapper.Map<List<OrderViewModel>>(orders);
            TempData["CarId"] = null;
            return View(model);
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _orderRepository.GetOrderByIdAsync(id.Value);
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Order/Create
        public async Task<IActionResult> Create(int carId)
        {
            if (!await PrepareCarViewDataAsync(carId))
                return NotFound();

            var car = await _carRepository.GetByIdAsync(carId);

            var viewModel = new OrderViewModel
            {
                CarId = car.CarId,
                Price = car.PricePerDay,
                Brand = car.Brand,
                Model = car.Model,
                CarDescription = car.CarDescription,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            TempData["CarId"] = carId;
            return View(viewModel);
        }

















        //POST: Order/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {


            if (!ModelState.IsValid)
            {
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("", "Du måste vara inloggad för att boka.");
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }

            // Kontrollera att slutdatum är efter startdatum
            int days = (viewModel.EndDate - viewModel.StartDate).Days;
            if (days <= 0)
            {
                ModelState.AddModelError("", "Slutdatum måste vara efter startdatum.");
                return View(viewModel);
            }

            var isBooked = await _orderRepository.IsCarBookedAsync(
                viewModel.CarId,
                viewModel.StartDate,
                viewModel.EndDate
            );

            if (isBooked)
            {
                TempData["TempData"] = "Bilen är tyvärr upptagen under denna tidsperiod.";
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }



            var car = await _carRepository.GetByIdAsync(viewModel.CarId);
            if (car == null)
            {
                ModelState.AddModelError("", "Bilen kunde inte hittas.");
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }

            var order = _mapper.Map<Order>(viewModel);
            order.UserId = user.Id;
            //order.ActiveOrder = true;
            order.Price = days * car.PricePerDay;

            await _orderRepository.AddOrderAsync(order);

            TempData["TempData"] = "Du har nu bokat bilen!";

            return User.IsInRole("Admin")
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Car");
        }


        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _orderRepository.GetOrderByIdAsync(id.Value);
            if (order == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<OrderViewModel>(order);

            var car = await _carRepository.GetByIdAsync(order.CarId);

            if (car != null)
            {
                viewModel.Brand = car.Brand;
                viewModel.Model = car.Model;
                viewModel.Year = car.Year;
                viewModel.CarDescription = car.CarDescription;
            }

            return View(viewModel);
        }



        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,StartDate,EndDate,Price,CarId,ActiveOrder, CarDescription, Model, Brand")] OrderViewModel viewModel)
        {
           
            if (id != viewModel.OrderId || !ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.StartDate >= viewModel.EndDate)
            {
                ModelState.AddModelError("", "Slutdatum måste vara efter startdatum.");
                return View(viewModel);
            }

            var order = await _orderRepository.GetOrderByIdAsync(viewModel.OrderId);

            if (order == null)
            {
                return NotFound();
            }

            var car = await _carRepository.GetByIdAsync(viewModel.CarId);


            if (car == null)
            {
                ModelState.AddModelError("CarId", "Bilen kunde inte hittas.");
                return View(viewModel);
            }

            _mapper.Map(viewModel, order);

            order.Car = car;

            {
                try
                {
                    await _orderRepository.UpdateOrderAsync(order);
                    TempData["TempData"] = "Ordern har uppdaterats";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _orderRepository.OrderExistsAsync(viewModel.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //if (await IfAdminAsync())
                //{
                //    return RedirectToAction("Index", "Admin");
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Order");
                //}

                return User.IsInRole("Admin")
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Order");

            }
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

           

            await _orderRepository.DeleteOrderAsync(id);

            //if (await IfAdminAsync())
            //{
            //    return RedirectToAction("Index", "Admin");
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Order");
            //}
            return User.IsInRole("Admin")
                ? RedirectToAction("Index", "Admin")
                : RedirectToAction("Index", "Order");
        }

        //private async Task<bool> IfAdminAsync()
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    var userRoles = await _userManager.GetRolesAsync(user);
        //    var isAdmin = userRoles.Contains("Admin");
        //    return isAdmin;
        //}

        private async Task<bool> PrepareCarViewDataAsync(int carId)
        {
            var car = await _carRepository.GetByIdAsync(carId);
            if (car == null) return false;

            ViewBag.PricePerDay = car.PricePerDay;
            ViewBag.CarInfo = $"{car.Brand} {car.Model} ({car.Year})";
            ViewBag.ImageUrls = car.CarImages.Select(i => i.Url).ToList();

            return true;
        }


    }
}