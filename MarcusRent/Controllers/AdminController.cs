// AdminController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MarcusRent.Classes;
using MarcusRent.Models;
using MarcusRent.Data; // Din IApplicationUserService
using MarcusRent.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MarcusRental2.Repositories;

namespace MarcusRent.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IApplicationUserService _userService;
        private readonly IMapper _mapper;

        public AdminController(
            ICarRepository carRepository,
            IOrderRepository orderRepository,
            IApplicationUserService userService,
            IMapper mapper)
        {
            _carRepository = carRepository;
            _orderRepository = orderRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllAsync();
            var orders = await _orderRepository.GetAllOrdersAsync();
            var users = await _userService.GetAllUsersAsync();

            var carViewModels = cars.Select(car =>
            {

                //var earnings = orders
                // .Where(o => o.CarId == car.CarId)
                //  .Sum(o => o.Price);

                var earnings = _orderRepository.GetTotalEarningsForCar(car.CarId);
                var activeRental = orders
                    .FirstOrDefault(o => o.CarId == car.CarId && o.EndDate > DateTime.Now);

                return new AdminCarViewModel
                {
                    CarId = car.CarId,
                    Brand = car.Brand,
                    Model = car.Model,
                    Year = car.Year,
                    Available = car.Available,
                    PricePerDay = car.PricePerDay,
                    ImageUrls = car.CarImages?.Select(img => img.Url).ToList() ?? new(),
                    TotalEarnings = earnings,
                    CurrentRentalEndDate = activeRental?.EndDate,
                    CurrentCustomerName = activeRental?.Customer?.FullName
                };
            }).ToList();

            var orderViewModels = orders.Select(o => new AdminOrderViewModel
            {
                OrderId = o.OrderId,
                CarName = $"{o.Car.Brand} {o.Car.Model}",
                CustomerName = o.Customer.FullName,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                Price = o.Price,
                ActiveOrder = o.EndDate > DateTime.Now
            }).ToList();

            var customerViewModels = users.Select(u => new AdminCustomerViewModel
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                ApprovedByAdmin = u.ApprovedByAdmin
            }).ToList();

            var vm = new AdminDashboardViewModel
            {
                Cars = carViewModels,
                Orders = orderViewModels,
                Customers = customerViewModels
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveCustomer(string id)
        {
            await _userService.ApproveUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}