using DependencyInjectionExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Data;

public class BookStoreContext : DbContext
{
    public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", Price = 12.99m, Category = "fiction", Stock = 50 },
            new Book { Id = 2, Title = "Clean Code", Author = "Robert C. Martin", Price = 34.99m, Category = "non-fiction", Stock = 30 },
            new Book { Id = 3, Title = "Dune", Author = "Frank Herbert", Price = 15.99m, Category = "fiction", Stock = 25 },
            new Book { Id = 4, Title = "Design Patterns", Author = "Gang of Four", Price = 44.99m, Category = "non-fiction", Stock = 20 },
            new Book { Id = 5, Title = "1984", Author = "George Orwell", Price = 10.99m, Category = "fiction", Stock = 40 },
            new Book { Id = 6, Title = "Refactoring", Author = "Martin Fowler", Price = 39.99m, Category = "non-fiction", Stock = 15 }
        );
    }
}
