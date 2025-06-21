using Microsoft.EntityFrameworkCore;
using MarcusRent.Classes;
using MarcusRent.Data;


namespace MarcusRental2.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.Customer)
                .ToListAsync();
        }
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }
        public async Task AddOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> IsCarBookedAsync(int carId, DateTime startDate, DateTime endDate)
        {
            return await _context.Orders.AnyAsync(o =>
                o.CarId == carId &&
                o.StartDate < endDate &&
                startDate < o.EndDate);
        }
        public async Task<bool> OrderExistsAsync(int id)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == id);
        }  
        public async Task<decimal> GetTotalEarningsForCarAsync(int id)
        {
            return await _context.Orders
                .Where(o => o.CarId == id)
                .SumAsync(o => o.Price);
        }
        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Include(o => o.Car)
                .Include(o=>o.Customer)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }
    }
}






