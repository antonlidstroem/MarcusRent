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
using static NuGet.Packaging.PackagingConstants;
using MarcusRent.Repositories;
using MarcusRental2.Repositories;
using System.Runtime.ConstrainedExecution;

namespace MarcusRent.Controllers
{
    public class OrderController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly ICarRepository _carRepository;


        public OrderController(UserManager<ApplicationUser> userManager, IMapper mapper, IOrderRepository orderRepository, ICarRepository carRepository)
        {
            //_context = context;
            _mapper = mapper;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _carRepository = carRepository;
        }

        //GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            var model = _mapper.Map<List<OrderViewModel>>(orders);
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
            //var cars = await _carRepository.GetAllAsync();
            var car = await _carRepository.GetByIdAsync(carId);

            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new OrderViewModel
            {
                CarId = car.CarId,
                Price = car.PricePerDay,
                Brand = car.Brand,
                Model = car.Model,
            };

            ViewBag.PricePerDay = car.PricePerDay;
            ViewBag.CarInfo = $"{car.Brand} {car.Model} ({car.Year})";

            return View(viewModel);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {

            if (!ModelState.IsValid)
            {
                DebugHelper.DebugModelStatePostCreate(ModelState);
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["TempData"] = "Du måste vara inloggad för att boka.";
                    return RedirectToAction("Login", "Account");
                }

                if (viewModel.StartDate >= viewModel.EndDate)
                {
                    TempData["TempData"] = "Slutdatum måste vara efter startdatum.";
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
                }
                else
                {

                    var order = _mapper.Map<Order>(viewModel);
                    order.UserId = user.Id;
                    order.ActiveOrder = true;


                    //var order = new Order
                    //{
                    //    CarId = viewModel.CarId,
                    //    StartDate = viewModel.StartDate,
                    //    EndDate = viewModel.EndDate,
                    //    Price = viewModel.Price,
                    //    ActiveOrder = true,
                    //    UserId = user.Id
                    //};

                    await _orderRepository.AddOrderAsync(order);
                    TempData["TempData"] = "Du har nu bokat bilen!";
                }

                return User.IsInRole("Admin")
               ? RedirectToAction("Index", "Admin")
               : RedirectToAction("Index", "Car");
            }
            return View(viewModel);
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

    // Mappa till ViewModel
    var viewModel = _mapper.Map<OrderViewModel>(order);

    // Fyll dropdown med bilar
    var cars = await _carRepository.GetAllAsync();
    viewModel.Cars = cars.Select(c => new SelectListItem
    {
        Value = c.CarId.ToString(),
        Text = $"{c.Brand} {c.Model}"
    }).ToList();

    return View(viewModel);
}



// POST: Order/Edit/5
// To protect from overposting attacks, enable the specific properties you want to bind to.
// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("OrderId,StartDate,EndDate,Price,CarId,ActiveOrder,Brand,Model,Year,CarName")] OrderViewModel viewModel)
{
    if (id != viewModel.OrderId || !ModelState.IsValid)
    {
        DebugHelper.DebugModelStatePostCreate(ModelState);
        return View(viewModel);
    }

    var order = await _orderRepository.GetOrderByIdAsync(viewModel.OrderId);
    //var car = await _carRepository.GetByIdAsync(viewModel.CarId);


    if (order == null)
    {
        return NotFound();
    }

    var car = await _carRepository.GetByIdAsync(viewModel.CarId);

    Console.WriteLine($"CarId from ViewModel: {viewModel.CarId}");


    if (car == null)
    {
        ModelState.AddModelError("CarId", "Bilen kunde inte hittas.");
        return View(viewModel);
    }

    _mapper.Map(viewModel, order);

    // Sätt Car-objektet explicit efter mapping (för att undvika EF-tracking problem)
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
        return RedirectToAction("Index", "Admin");
    }
}













//// GET: Order/Delete/5
//public async Task<IActionResult> Delete(int? id)
//{
//    if (id == null)
//    {
//        return NotFound();
//    }


//    var order = await _orderRepository.GetOrderByIdAsync(id.Value);

//    if (order == null)
//    {
//        return NotFound();
//    }

//    return View(order);
//}

// POST: Order/Delete/5
[HttpPost, ActionName("DeleteConfirmed")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var order = await _orderRepository.GetOrderByIdAsync(id);

    if (order == null)
        return NotFound();

    if (order != null)
        try
        {
            await _orderRepository.DeleteOrderAsync(id);
            return RedirectToAction("Index", "Admin");

        }
        catch (DbUpdateException)
        {
            return RedirectToAction("Index", "Admin");
        }

    return RedirectToAction(nameof(Index));
}
    }
}