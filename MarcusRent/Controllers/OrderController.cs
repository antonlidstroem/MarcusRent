using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MarcusRent.Classes;
using MarcusRent.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MarcusRent.Repositories;
using MarcusRental2.Repositories;


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

            var orders = isAdmin
            ? await _orderRepository.GetAllOrdersAsync()
            : await _orderRepository.GetOrdersByUserIdAsync(user.Id);


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

            var viewModel = _mapper.Map<OrderViewModel>(car);
            viewModel.StartDate = DateTime.Today;
            viewModel.EndDate = DateTime.Today.AddDays(1);


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
                TempData["TempData"] = "Du måste vara inloggad för att boka.";
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }

            if (!user.ApprovedByAdmin)
            {
                TempData["TempData"] = "Din användare är inte godkänd av administratören för att boka bilar.";
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }

            // KONTROLLERA SLUTDATUM
            var endDateValidationResult = await ValidateEndDateAsync(viewModel);
            if (endDateValidationResult != null)
                return endDateValidationResult;

            // KONTROLLERA OM BILEN FINNS
            var car = await _carRepository.GetByIdAsync(viewModel.CarId);
            var carExistValidationResult = await DoesCarExistAsync(viewModel, car);
            if (carExistValidationResult != null)
                return carExistValidationResult;

            // KONTROLLERA OM BILEN REDAN ÄR BOKAD
            var availabilityValidationResult = await ValidateCarAvailabilityAsync(viewModel);
            if (availabilityValidationResult != null)
                return availabilityValidationResult;
            
            // BERÄKNA PRIS
            var days = CountDaysDifference(viewModel);
            var order = _mapper.Map<Order>(viewModel);
            order.UserId = user.Id;
            order.Price = days * car.PricePerDay;

            await _orderRepository.AddOrderAsync(order);

            TempData["TempData"] = "Du har nu bokat bilen!";
            return RedirectBasedOnRole(userController: "Car");
        }



        private async Task<IActionResult?> DoesCarExistAsync(OrderViewModel viewModel, Car? car)
        {
            if (car == null)
            {
                TempData["TempData"] = "Bilen kunde inte hittas.";
                await PrepareCarViewDataAsync(viewModel.CarId);
                return RedirectToAction("Index", "Order");

            }
            return null;
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

            return View(viewModel);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel viewModel)
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
            return RedirectBasedOnRole();
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
            return RedirectBasedOnRole();
        }

        //UPPDATERAR VIEWBAG 
        private async Task<bool> PrepareCarViewDataAsync(int carId)
        {
            var car = await _carRepository.GetByIdAsync(carId);
            if (car == null) return false;

            ViewBag.PricePerDay = car.PricePerDay;
            ViewBag.CarInfo = $"{car.Brand} {car.Model} ({car.Year})";
            ViewBag.ImageUrls = car.CarImages.Select(i => i.Url).ToList();

            return true;
        }

        private IActionResult RedirectBasedOnRole(string adminAction = "Index", string adminController = "Admin", string userAction = "Index", string userController = "Order")
        {
            return User.IsInRole("Admin")
                ? RedirectToAction(adminAction, adminController)
                : RedirectToAction(userAction, userController);
        }
        private async Task<IActionResult?> ValidateEndDateAsync(OrderViewModel viewModel)
        {
            if (CountDaysDifference(viewModel) <= 0)
            {
                TempData["TempData"] = "Slutdatum måste vara efter startdatum.";
                await PrepareCarViewDataAsync(viewModel.CarId);
                return View(viewModel);
            }
            return null;
        }

        private int CountDaysDifference(OrderViewModel viewModel)
        {
            int days = (viewModel.EndDate - viewModel.StartDate).Days;
            return days;
        }
        private async Task<IActionResult?> ValidateCarAvailabilityAsync(OrderViewModel viewModel)
        {
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
            return null;
        }
    }
}