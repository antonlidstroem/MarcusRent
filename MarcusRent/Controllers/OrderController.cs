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
        public async Task<IActionResult> Create()
        {
            var cars = await _carRepository.GetAllAsync();

            var viewModel = new OrderViewModel
            {
                Cars = cars.Select(c => new SelectListItem
                {
                    Value = c.CarId.ToString(),
                    Text = c.Brand + " " + c.Model
                }).ToList()
            };

            return View(viewModel);
        }



        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {
            //DebugHelper.DebugModelStatePostCreate(ModelState);

            if (!ModelState.IsValid)
            {
                var cars = await _carRepository.GetAllAsync();
                viewModel.Cars = cars.Select(c => new SelectListItem
                {
                    Value = c.CarId.ToString(),
                    Text = c.Brand + " " + c.Model
                }).ToList();
                return View(viewModel);
            }

            var isBooked = await _orderRepository.IsCarBookedAsync(viewModel.CarId, viewModel.StartDate, viewModel.EndDate);
            if (isBooked)
            {
                ModelState.AddModelError("", "Den här bilen är redan bokad under den valda perioden.");
                var cars = await _carRepository.GetAllAsync();
                viewModel.Cars = cars.Select(c => new SelectListItem
                {
                    Value = c.CarId.ToString(),
                    Text = c.Brand + " " + c.Model
                }).ToList();
                return View(viewModel);
            }

            var order = _mapper.Map<Order>(viewModel);
            order.UserId = _userManager.GetUserId(User);

            await _orderRepository.AddOrderAsync(order);

            TempData["BookedCar"] = "Du har nu bokat bilen!";

            return RedirectToAction(nameof(Index));
        }


        // GET: Order/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,StartDate,EndDate,Price,ActiveOrder")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _orderRepository.UpdateOrderAsync(order);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _orderRepository.OrderExistsAsync(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order != null)
            {
                await _orderRepository.DeleteOrderAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

     

        //private async Task<bool> OrderExists(int id)
        //{
        //    return await _orderRepository.OrderExistsAsync(id);
        //}

    }
}
