using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Classes;
using MarcusRent.Models;
using AutoMapper;
using DAL.Repositories;


namespace MarcusRent.Controllers
{
    public class CarController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICarRepository _carRepository;

        public CarController(IMapper mapper, ICarRepository carRepository)
        {

            _mapper = mapper;
            _carRepository = carRepository;
        }
        public async Task<IActionResult> Index()
        {
            var availableCars = await _carRepository.GetAllAvailableAsync();
            var model = _mapper.Map<List<CarViewModel>>(availableCars);
            return View(model);
        }


        // GET: Car/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _carRepository.GetByIdAsync(id.Value);
            if (car == null) return NotFound();

            return View(car);
        }
        //GET: Car/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Car/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var car = _mapper.Map<Car>(viewModel);

            UpdateCarImages(car, viewModel.ImageUrls);

            await _carRepository.AddAsync(car);
            return RedirectToAction("Index", "Admin");
        }

        // GET: Car/Edit/5
        //[HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var car = await _carRepository.GetByIdAsync(id.Value);
            if (car == null)
                return NotFound();

            var model = _mapper.Map<CarViewModel>(car);

            model.ImageUrls = car.CarImages?.Select(ci => ci.Url).ToList() ?? new List<string>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarViewModel viewModel)
        {
            DebugHelper.DebugModelStatePostCreate(ModelState);

            if (!ModelState.IsValid)
                return View(viewModel);

            var carEntity = await _carRepository.GetByIdAsync(viewModel.CarId);
            if (carEntity == null)
                return NotFound();

            _mapper.Map(viewModel, carEntity);

            carEntity.CarImages.Clear();

            UpdateCarImages(carEntity, viewModel.ImageUrls);

           

            try
            {
                await _carRepository.UpdateAsync(carEntity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _carRepository.ExistsAsync(viewModel.CarId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction("Index", "Admin");
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
                return NotFound();

            try
            {
                await _carRepository.DeleteAsync(id);
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Admin");

            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Bilen kan inte tas bort eftersom den är kopplad till ordrar.";
                return RedirectToAction("Index", "Admin");
            }
        }

        private void UpdateCarImages(Car car, List<string>? imageUrls)
        {
            car.CarImages.Clear();

            if (imageUrls == null) return;

            foreach (var url in imageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
            {
                car.CarImages.Add(new CarImage { Url = url });
            }
        }

    }
}


