using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MarcusRent.Classes;
using MarcusRental2.Repositories;


namespace MarcusRent.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;




        public UserController(UserManager<ApplicationUser> userManager, IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
        }

        // GET: Users/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            TempData["CarId"] = null;

            return View(user); // skapa en vy för detta
        }

        // POST: Users/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser updatedUser)
        {

            TempData["CarId"] = null;

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.Email; 
            user.ApprovedByAdmin = updatedUser.ApprovedByAdmin;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index", "Admin");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);


            return View(user);
        }

        // POST: Users/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            TempData["CarId"] = null;

            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "ID saknas!";
                return RedirectToAction("Index", "Admin");
            }

            // Hämta användaren med userId (inte e-post)
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = $"Ingen användare med id {userId} hittades.";
                return RedirectToAction("Index", "Admin");
            }

            // Kontrollera om användaren har ordrar
            var orders = await _orderRepository.GetOrdersByUserIdAsync(user.Id);
            if (orders.Any())
            {
                TempData["ErrorMessage"] = "Användaren kan inte tas bort eftersom den har kopplade ordrar.";
                return RedirectToAction("Index", "Admin");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Det gick inte att ta bort användaren.";
                return RedirectToAction("Index", "Admin");
            }

            TempData["TempData"] = "Användaren har nu tagits bort!";
            return RedirectToAction("Index", "Admin");
        }


    }
}


    

   


