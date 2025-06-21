using MarcusRent.Classes;
using Microsoft.AspNetCore.Identity;

namespace MarcusRent.Repositories
{
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