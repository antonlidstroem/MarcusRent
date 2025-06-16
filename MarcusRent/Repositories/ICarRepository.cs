using MarcusRent.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarcusRent.Repositories
{
    public interface ICarRepository
    {
        Task<List<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task AddAsync(Car car);
        Task UpdateAsync(Car car);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

