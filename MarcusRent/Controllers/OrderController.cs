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

namespace MarcusRent.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }



        //GET: Order
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
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

        public IActionResult Create()
        {
            var viewModel = new OrderViewModel
            {
                CarOptions = _context.Cars.Select(c => new SelectListItem
                {
                    Value = c.CarId.ToString(),
                    Text = $"{c.Brand} {c.Model}"
                }).ToList(),

                UserOptions = _context.Users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                }).ToList()
            };

            return View(viewModel);
        }




        // GET: Order/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("OrderId,StartDate,EndDate,Price,ActiveOrder")] Order order)


        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(order);
        //}

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Mappa ViewModel till Order-entitet
                var order = _mapper.Map<Order>(viewModel);

  
                // Här kopplar du ihop navigation propertyn
                order.Customer = await _context.Users.FirstOrDefaultAsync(u => u.Id == viewModel.UserId);

                // Du *behöver inte* sätta order.UserId = viewModel.UserId manuellt, EF sätter det när du tilldelar Customer.
                // Men du *kan* sätta båda om du vill vara tydlig:
                order.UserId = viewModel.UserId;


                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Skapa koppling till bil via CarOrder
                var carOrder = new CarOrder
                {
                    CarId = viewModel.CarId,
                    OrderId = order.OrderId
                };
                _context.CarOrders.Add(carOrder);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Om validering misslyckas: fyll dropdowns igen
            viewModel.CarOptions = _context.Cars.Select(c => new SelectListItem
            {
                Value = c.CarId.ToString(),
                Text = $"{c.Brand} {c.Model}"
            });

            viewModel.UserOptions = _context.Users.Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.Email
            });

            return View(viewModel);
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
