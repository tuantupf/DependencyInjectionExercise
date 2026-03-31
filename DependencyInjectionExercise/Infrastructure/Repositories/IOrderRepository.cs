using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Infrastructure.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task<List<Order>> GetAllAsync();
        Task<int> CountByCustomerAsync(string customerName);
        Task SaveChangesAsync();
    }
}
