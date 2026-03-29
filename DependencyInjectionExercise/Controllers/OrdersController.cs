using DependencyInjectionExercise.Data;
using DependencyInjectionExercise.Models;
using DependencyInjectionExercise.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly BookStoreContext _context;
    private readonly DiscountService _discountService;
    private readonly OrderTrackingService _orderTracking;
    private readonly NotificationHub _notificationHub;

    public OrdersController(
        BookStoreContext context,
        DiscountService discountService,
        OrderTrackingService orderTracking,
        NotificationHub notificationHub)
    {
        _context = context;
        _discountService = discountService;
        _orderTracking = orderTracking;
        _notificationHub = notificationHub;
    }

    [HttpPost]
    public async Task<ActionResult<Order>> PlaceOrder(Order order)
    {
        var book = await _context.Books.FindAsync(order.BookId);
        if (book == null)
            return NotFound("Book not found");

        if (order.Quantity <= 0)
            return BadRequest("Quantity must be greater than zero");

        if (book.Stock < order.Quantity)
            return BadRequest($"Not enough stock. Available: {book.Stock}");

        order.TotalPrice = book.Price * order.Quantity;

        var discountPercent = _discountService.CalculateDiscount(
            book.Category, order.Quantity, order.CustomerName);
        order.DiscountApplied = discountPercent;
        order.TotalPrice *= (1 - discountPercent);

        book.Stock -= order.Quantity;
        order.OrderDate = DateTime.UtcNow;
        order.Status = "confirmed";

        _orderTracking.SetTrackingNote($"Order placed for {order.Quantity}x '{book.Title}'");

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        var secondTracking = HttpContext.RequestServices.GetRequiredService<OrderTrackingService>();
        var trackingNote = secondTracking.GetTrackingNote();
        order.TrackingNote = trackingNote;
        await _context.SaveChangesAsync();

        SendNotification(order, book,
            $"Your order for {order.Quantity}x '{book.Title}' has been confirmed. Total: ${order.TotalPrice:F2}");

        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        return order;
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetOrders()
    {
        return await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        if (order.Status == "cancelled")
            return BadRequest("Order is already cancelled");

        var book = await _context.Books.FindAsync(order.BookId);
        if (book != null)
            book.Stock += order.Quantity;

        order.Status = "cancelled";
        await _context.SaveChangesAsync();

        SendNotification(order, book,
            $"Your order #{order.Id} has been cancelled. Refund: ${order.TotalPrice:F2}");

        return NoContent();
    }

    [HttpGet("debug/discount-state")]
    public ActionResult GetDiscountDebugState()
    {
        return Ok(new
        {
            _discountService.LastCustomerName,
            _discountService.LastAppliedDiscount,
            _discountService.OrderCountInSession
        });
    }

    [HttpGet("debug/tracking-instance")]
    public ActionResult GetTrackingDebugState()
    {
        return Ok(new
        {
            _orderTracking.InstanceId,
            TrackingNote = _orderTracking.GetTrackingNote(),
            Warning = "If TrackingNote is null, that's a bug!"
        });
    }

    private void SendNotification(Order order, Book? book, string message)
    {
        if (order.NotificationMethod == "email")
        {
            Console.WriteLine($"[EMAIL] To: {order.CustomerEmail} | {message}");
            _notificationHub.Add(new NotificationLog
            {
                Timestamp = DateTime.UtcNow,
                Channel = "email",
                Recipient = order.CustomerEmail,
                Message = message
            });
        }
        else if (order.NotificationMethod == "sms")
        {
            Console.WriteLine($"[SMS] To: {order.CustomerPhone} | {message}");
            _notificationHub.Add(new NotificationLog
            {
                Timestamp = DateTime.UtcNow,
                Channel = "sms",
                Recipient = order.CustomerPhone,
                Message = message
            });
        }
        else
        {
            Console.WriteLine($"[UNKNOWN CHANNEL: {order.NotificationMethod}] {message}");
        }
    }
}
