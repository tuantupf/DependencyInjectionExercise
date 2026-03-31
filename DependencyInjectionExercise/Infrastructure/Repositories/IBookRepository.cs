using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Infrastructure.Repositories
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(int id);
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> GetByCategoryAsync(string category);
        Task AddAsync(Book book);
        Task SaveChangesAsync();
    }
}
