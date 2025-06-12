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

namespace MarcusRent.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }








        //GET: Order
      
           public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)  // Hämtar med kundinfo
                .Include(c => c.Car)
                .ToListAsync();

            var model = _mapper.Map<List<OrderViewModel>>(orders);
            return View(model);
        }









        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);


            if (id == null)
            {
                return NotFound();
            }

            return View(order);


        }

        // GET: Order/Create?carId=123
        //[Authorize]
        public IActionResult Create(int carId)
        {
            if (!User.Identity.IsAuthenticated || User.Identity == null)
            {
                TempData["Message"] = "Du måste vara inloggad för att boka en bil.";
                return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = $"/Order/Create?carId={carId}" });
            }


            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);
            if (car == null)
            {
                return NotFound();
            }

            ViewBag.CarInfo = $"{car.Brand} {car.Model} ({car.Year}) \n\n {car.PricePerDay} kr/dag";
            ViewBag.PricePerDay = car.PricePerDay;

            var viewModel = new OrderViewModel
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                CarId = car.CarId,
                //Description = $"{car.Brand} {car.Model} ({car.Year})"

            };

            
            return View(viewModel);
        }










        //[Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {


            DebugModelStatePostCreate();

            if (!ModelState.IsValid)
            {
                viewModel.Cars = _context.Cars
                    .Select(c => new SelectListItem
                    {
                        Value = c.CarId.ToString(),
                        Text = c.Brand + " " + c.Model
                    }).ToList();

                return View(viewModel);
            }

            // Kontrollera om bilen redan är bokad under perioden
            bool isBooked = await _context.Orders.AnyAsync(o =>
                o.CarId == viewModel.CarId &&
                o.StartDate < viewModel.EndDate &&
                viewModel.StartDate < o.EndDate);

            if (isBooked)
            {
                ModelState.AddModelError("", "Den här bilen är redan bokad under den valda perioden.");
                viewModel.Cars = _context.Cars
                    .Select(c => new SelectListItem
                    {
                        Value = c.CarId.ToString(),
                        Text = c.Brand + " " + c.Model
                    }).ToList();

                return View(viewModel);
            }

            var order = _mapper.Map<Order>(viewModel);
            order.UserId = _userManager.GetUserId(User);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        //FELSÖKNING
        private void DebugModelStatePostCreate()
        {
            foreach (var entry in ModelState)
            {
                var key = entry.Key;
                var errors = entry.Value.Errors;

                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState error for {key}: {error.ErrorMessage}");
                }
            }
        }



        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
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
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == id);
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
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
