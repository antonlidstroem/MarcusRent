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
using MarcusRent.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MarcusRent.Controllers
{
    public class CarController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICarRepository _carRepository;

        //public CarController(ApplicationDbContext context, IMapper mapper, ICarRepository carRepository)
        public CarController(IMapper mapper, ICarRepository carRepository)
        {
            //_context = context;
            _mapper = mapper;
            _carRepository = carRepository;
        }

        // GET: Car
        public async Task<IActionResult> Index()
        {
            var cars = await _carRepository.GetAllAsync();

            var model = _mapper.Map<List<CarViewModel>>(cars);

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

            DebugHelper.DebugModelStatePostCreate(ModelState);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var car = _mapper.Map<Car>(viewModel);

            if (viewModel.ImageUrls != null)
            {
                var validImageUrls = viewModel.ImageUrls
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .ToList();

                if (validImageUrls.Any())
                {
                    car.CarImages = validImageUrls.Select(url => new CarImage { Url = url }).ToList();
                }
            }

            await _carRepository.AddAsync(car);

            return RedirectToAction(nameof(Index));
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

            // Mappa bilder separat om du inte redan gör det via AutoMapper
            model.ImageUrls = car.CarImages?.Select(ci => ci.Url).ToList() ?? new List<string>();

            return View(model);
        }








        // POST: Car/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int? id, CarViewModel model)
        //{
        //    if (id != model.CarId)
        //        return NotFound();

        //    if (!ModelState.IsValid)
        //        return View(model);

        //    //var carEntity = await _context.Cars.FindAsync(id);
        //    var carEntity = await _carRepository.GetByIdAsync(id.Value);
        //    if (carEntity == null)
        //        return NotFound();

        //    _mapper.Map(model, carEntity);

        //    try
        //    {
        //        await _carRepository.UpdateAsync(carEntity);
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!await _carRepository.ExistsAsync(id.Value))
        //            return NotFound();
        //        else
        //            throw;
        //    }

        //    return RedirectToAction(nameof(Index));
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarViewModel ViewModel)
        {
            DebugHelper.DebugModelStatePostCreate(ModelState);

            if (!ModelState.IsValid)
                return View(ViewModel);

            var carEntity = await _carRepository.GetByIdAsync(ViewModel.CarId);
            if (carEntity == null)
                return NotFound();

            _mapper.Map(ViewModel, carEntity);

            carEntity.CarImages.Clear();

            if (ViewModel.ImageUrls != null)
            {
                foreach (var url in ViewModel.ImageUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    carEntity.CarImages.Add(new CarImage { Url = url });
                }
            }

            try
            {
                await _carRepository.UpdateAsync(carEntity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _carRepository.ExistsAsync(ViewModel.CarId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction("Index", "Admin");
        }


























        // GET: Car/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        await _carRepository.DeleteAsync(id);
        //        return Json(new { success = true });
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Anta att det handlar om en FK-konflikt med Orders
        //        return Json(new { success = false, message = "Bilen kan inte tas bort eftersom den är registrerad i en eller flera ordrar." });
        //    }
        //}









        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        await _carRepository.DeleteAsync(id);
        //    }
        //    catch (DbUpdateException)
        //    {
        //        return BadRequest("Bilen kunde inte tas bort. Den är kopplad till en eller flera ordrar.");
        //    }

        //    return Ok();
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        
        //{
        //    var car = await _carRepository.GetByIdAsync(id);
        //    if (car == null)
        //    {
        //        return Json(new { success = false, message = "Car not found" });
        //    }

        //   await _carRepository.DeleteAsync(id);

        //    return Json(new { success = true });
        //}



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
                ModelState.AddModelError("", "Bilen kan inte tas bort eftersom den är kopplad till ordrar.");
                var model = _mapper.Map<CarViewModel>(car);
                return View("Delete", model);
            }
        }


    }
}


