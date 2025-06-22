//using System.Collections.Generic;
//using System.Threading.Tasks;
using MarcusRent.Classes;


namespace MarcusRent.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task<bool> IsCarBookedAsync(int carId, DateTime startDate, DateTime endDate);
        Task<bool> OrderExistsAsync(int id);

        Task<decimal> GetTotalEarningsForCarAsync(int id);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

    }
}
