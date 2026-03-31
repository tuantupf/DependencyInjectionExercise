using DependencyInjectionExercise.Application;
using DependencyInjectionExercise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    //private readonly BookStoreContext _context;
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    //[HttpGet]
    //public async Task<ActionResult<List<Book>>> GetBooks()
    //{
    //    return await _context.Books.ToListAsync();
    //}
    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetBooks()
    {
        return Ok(await _bookService.GetBooksAsync());
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<Book>> GetBook(int id)
    //{
    //    var book = await _context.Books.FindAsync(id);
    //    if (book == null) return NotFound();
    //    return book;
    //}
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null) return NotFound();
        return Ok(book);
    }

    //[HttpGet("category/{category}")]
    //public async Task<ActionResult<List<Book>>> GetBooksByCategory(string category)
    //{
    //    if (category != "fiction" && category != "non-fiction")
    //        return BadRequest("Category must be 'fiction' or 'non-fiction'");

    //    return await _context.Books.Where(b => b.Category == category).ToListAsync();
    //}
    [HttpGet("category/{category}")]
    public async Task<ActionResult<List<Book>>> GetBooksByCategory(string category)
    {
        try
        {
            return Ok(await _bookService.GetBooksByCategoryAsync(category));
        }
        catch (Exception ex) { 
            return BadRequest(ex.Message);
        }
    }

    //[HttpPost]
    //public async Task<ActionResult<Book>> CreateBook(Book book)
    //{
    //    if (string.IsNullOrWhiteSpace(book.Title))
    //        return BadRequest("Title is required");
    //    if (book.Price <= 0)
    //        return BadRequest("Price must be greater than zero");
    //    if (book.Stock < 0)
    //        return BadRequest("Stock cannot be negative");

    //    _context.Books.Add(book);
    //    await _context.SaveChangesAsync();
    //    return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    //}
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        try
        {
            var createdBook = await _bookService.CreateBookAsync(book);

            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpPut("{id}/stock")]
    //public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    //{
    //    var book = await _context.Books.FindAsync(id);
    //    if (book == null) return NotFound();

    //    if (book.Stock + quantity < 0)
    //        return BadRequest("Not enough stock");

    //    book.Stock += quantity;
    //    await _context.SaveChangesAsync();
    //    return NoContent();
    //}
    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        try
        {
            var success = await _bookService.UpdateStockAsync(id, quantity);
            if (!success) return NotFound();
            return NoContent();

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
