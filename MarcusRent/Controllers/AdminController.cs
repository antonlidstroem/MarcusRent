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
        private readonly IApplicationUserRepository _userService;
        private readonly IMapper _mapper;

        public AdminController(
            ICarRepository carRepository,
            IOrderRepository orderRepository,
            IApplicationUserRepository userService,
            IMapper mapper)
        {
            _carRepository = carRepository;
            _orderRepository = orderRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            TempData["CarId"] = null;
            var cars = await _carRepository.GetAllAsync();
            var orders = await _orderRepository.GetAllOrdersAsync();
            var users = await _userService.GetAllUsersAsync();

            // Map Car -> AdminCarViewModel
            var carViewModels = _mapper.Map<List<CarViewModel>>(cars);

            foreach (var carVM in carViewModels)
            {
                var earnings = await _orderRepository.GetTotalEarningsForCarAsync(carVM.CarId);
                var activeRental = orders
                    .FirstOrDefault(o => o.CarId == carVM.CarId && o.EndDate > DateTime.Now);

                carVM.TotalEarnings = earnings;
                carVM.CurrentRentalEndDate = activeRental?.EndDate;
                carVM.CurrentCustomerName = activeRental?.Customer?.FullName;
            }

            // Map Order -> AdminOrderViewModel
            var orderViewModels = _mapper.Map<List<OrderViewModel>>(orders);

            // Map ApplicationUser -> AdminCustomerViewModel
            var customerViewModels = _mapper.Map<List<CustomerViewModel>>(users);

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