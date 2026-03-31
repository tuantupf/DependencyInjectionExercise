using DependencyInjectionExercise.Infrastructure.Data;
using DependencyInjectionExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookStoreContext _context;

        public BookRepository(BookStoreContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<List<Book>> GetByCategoryAsync(string category)
        {
            return await _context.Books
                .Where(b => b.Category == category)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
