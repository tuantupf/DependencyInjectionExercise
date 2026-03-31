using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Application
{
    public interface IBookService
    {
        Task<List<Book>> GetBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> GetBooksByCategoryAsync(string category);
        Task<Book> CreateBookAsync(Book book);
        Task<bool> UpdateStockAsync(int id, int quantity);
    }
}
