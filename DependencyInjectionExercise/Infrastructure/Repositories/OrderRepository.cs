using DependencyInjectionExercise.Infrastructure.Data;
using DependencyInjectionExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BookStoreContext _context;
        
        public OrderRepository(BookStoreContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<int> CountByCustomerAsync(string customerName)
        {
            return await _context.Orders.CountAsync(o => o.CustomerName == customerName);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
