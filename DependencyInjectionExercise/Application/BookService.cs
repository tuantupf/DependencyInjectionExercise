using DependencyInjectionExercise.Infrastructure.Repositories;
using DependencyInjectionExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Application
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title))
                throw new Exception("Title is required");
            if (book.Price <= 0)
                throw new Exception("Price must be greater than zero");
            if (book.Stock < 0)
                throw new Exception("Stock cannot be negative");

            await _bookRepository.AddAsync(book);
            await _bookRepository.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<List<Book>> GetBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<List<Book>> GetBooksByCategoryAsync(string category)
        {
            if (category != "fiction" && category != "non-fiction")
                throw new ArgumentException("Category must be 'fiction' or 'non-fiction'");

            return await _bookRepository.GetByCategoryAsync(category);
        }

        public async Task<bool> UpdateStockAsync(int id, int quantity)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return false;

            if (book.Stock + quantity < 0)
                throw new Exception("Not enough stock");

            book.Stock += quantity;
            await _bookRepository.SaveChangesAsync();
            return true;
        }
    }
}
