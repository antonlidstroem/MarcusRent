using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/cars")]
    [Authorize]
    public class CarsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetCars()
        {
            var cars = new[]
            {
            new { CarId = 1, Brand = "Volvo", Model = "XC90", Year = 2020 },
            new { CarId = 2, Brand = "Tesla", Model = "Model 3", Year = 2022 }
        };

            return Ok(cars);
        }
    }
}
