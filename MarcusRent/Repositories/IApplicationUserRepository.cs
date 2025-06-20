using MarcusRent.Classes;
using Microsoft.AspNetCore.Identity;

namespace MarcusRent.Repositories
{
    // 1. Skapa ett gränssnitt för Identity-användare som admin kan se och hantera:

    public interface IApplicationUserRepository
    {
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task ApproveUserAsync(string id);
        Task DeleteUserAsync(string id);
        Task UpdateUserAsync(ApplicationUser user);
        Task<ApplicationUser?> AddAsync(string firstName, string lastName, string email, string password, string role);
    }
}
    // 2. Implementera detta:

//    public class ApplicationUserService : IApplicationUserService
//    {
//        private readonly UserManager<ApplicationUser> _userManager;

//        public ApplicationUserService(UserManager<ApplicationUser> userManager)
//        {
//            _userManager = userManager;
//        }

//        public async Task<List<ApplicationUser>> GetAllUsersAsync()
//        {
//            return _userManager.Users.ToList();
//        }

//        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
//        {
//            return await _userManager.FindByIdAsync(id);
//        }

//        public async Task ApproveUserAsync(string id)
//        {
//            var user = await _userManager.FindByIdAsync(id);
//            if (user != null)
//            {
//                user.ApprovedByAdmin = true;
//                await _userManager.UpdateAsync(user);
//            }
//        }

//        public async Task DeleteUserAsync(string id)
//        {
//            var user = await _userManager.FindByIdAsync(id);
//            if (user != null)
//            {
//                await _userManager.DeleteAsync(user);
//            }
//        }

//        public async Task UpdateUserAsync(ApplicationUser user)
//        {
//            await _userManager.UpdateAsync(user);
//        }
//    }

//}