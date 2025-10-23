using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;


namespace MarcusRent.Classes
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var client = _httpClientFactory.CreateClient();

            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:5001/api/auth/login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseString);

                // Spara token i session eller cookie (exempel med session):
                HttpContext.Session.SetString("JWToken", loginResponse.Token);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Felaktigt användarnamn eller lösenord");
            return View();
        }
    }
}
