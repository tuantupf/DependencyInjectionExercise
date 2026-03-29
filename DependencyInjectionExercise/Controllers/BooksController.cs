using DependencyInjectionExercise.Data;
using DependencyInjectionExercise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookStoreContext _context;

    public BooksController(BookStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetBooks()
    {
        return await _context.Books.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();
        return book;
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<Book>>> GetBooksByCategory(string category)
    {
        if (category != "fiction" && category != "non-fiction")
            return BadRequest("Category must be 'fiction' or 'non-fiction'");

        return await _context.Books.Where(b => b.Category == category).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
            return BadRequest("Title is required");
        if (book.Price <= 0)
            return BadRequest("Price must be greater than zero");
        if (book.Stock < 0)
            return BadRequest("Stock cannot be negative");

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        if (book.Stock + quantity < 0)
            return BadRequest("Not enough stock");

        book.Stock += quantity;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
