using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using MarcusRent.Classes;
using MarcusRent.Models;
using MarcusRent.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace MarcusRent.Controllers
{
    public class CarController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

       
        public CarController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient GetClientWithToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return null!; // Hantera i anrop

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
        public async Task<IActionResult> Index()
        {
            var client = GetClientWithToken();
            if (client == null)
                return RedirectToAction("Login", "Account");

            var response = await client.GetAsync("https://localhost:5001/api/cars");
            if (!response.IsSuccessStatusCode)
                return Unauthorized();

            var json = await response.Content.ReadAsStringAsync();
            var cars = JsonSerializer.Deserialize<List<CarViewModel>>(json, _jsonOptions);
            return View(cars);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = GetClientWithToken();
            if (client == null)
                return RedirectToAction("Login", "Account");

            var response = await client.GetAsync($"https://localhost:5001/api/cars/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var car = JsonSerializer.Deserialize<CarViewModel>(json, _jsonOptions);
            return View(car);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var client = GetClientWithToken();
            if (client == null)
                return RedirectToAction("Login", "Account");

            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:5001/api/cars", jsonContent);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Något gick fel vid skapandet av bilen.");
            return View(viewModel);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = GetClientWithToken();
            if (client == null)
                return RedirectToAction("Login", "Account");

            var response = await client.GetAsync($"https://localhost:5001/api/cars/{id}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            var car = JsonSerializer.Deserialize<CarViewModel>(json, _jsonOptions);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarViewModel viewModel)
        {
            if (id != viewModel.CarId)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(viewModel);

            var client = GetClientWithToken();
            if (client == null)
                return RedirectToAction("Login", "Account");

            var jsonContent = new StringContent(JsonSerializer.Serialize(viewModel), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:5001/api/cars/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Något gick fel vid uppdateringen av bilen.");
            return View(viewModel);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = GetClientWithToken();
            if (client == null)
                return RedirectToAction("Login", "Account");

            var response = await client.DeleteAsync($"https://localhost:5001/api/cars/{id}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            TempData["ErrorMessage"] = "Bilen kunde inte tas bort.";
            return RedirectToAction(nameof(Index));
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


