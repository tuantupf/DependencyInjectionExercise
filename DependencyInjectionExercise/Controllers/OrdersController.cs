using DependencyInjectionExercise.Application;
using DependencyInjectionExercise.Infrastructure.Discounts;
using DependencyInjectionExercise.Infrastructure.Tracking;
using DependencyInjectionExercise.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    //private readonly BookStoreContext _context;
    private readonly DiscountService _discountService;
    private readonly OrderTrackingService _orderTracking;
    //private readonly NotificationService _notificationService;
    private readonly IOrderService _orderService;

    public OrdersController(
        DiscountService discountService,
        OrderTrackingService orderTracking,
        IOrderService orderService)
    {
        _discountService = discountService;
        _orderTracking = orderTracking;
        _orderService = orderService;
    }

    //[HttpPost]
    //public async Task<ActionResult<Order>> PlaceOrder(Order order)
    //{
    //    var book = await _context.Books.FindAsync(order.BookId);
    //    if (book == null)
    //        return NotFound("Book not found");

    //    if (order.Quantity <= 0)
    //        return BadRequest("Quantity must be greater than zero");

    //    if (book.Stock < order.Quantity)
    //        return BadRequest($"Not enough stock. Available: {book.Stock}");

    //    order.TotalPrice = book.Price * order.Quantity;

    //    var discountPercent = _discountService.CalculateDiscount(
    //        book.Category, order.Quantity, order.CustomerName);
    //    order.DiscountApplied = discountPercent;
    //    order.TotalPrice *= (1 - discountPercent);

    //    book.Stock -= order.Quantity;
    //    order.OrderDate = DateTime.UtcNow;
    //    order.Status = "confirmed";

    //    _orderTracking.SetTrackingNote($"Order placed for {order.Quantity}x '{book.Title}'");

    //    _context.Orders.Add(order);
    //    await _context.SaveChangesAsync();

    //    var secondTracking = HttpContext.RequestServices.GetRequiredService<OrderTrackingService>();
    //    var trackingNote = secondTracking.GetTrackingNote();
    //    order.TrackingNote = trackingNote;
    //    await _context.SaveChangesAsync();

    //    //SendNotification(order, book,
    //    //    $"Your order for {order.Quantity}x '{book.Title}' has been confirmed. Total: ${order.TotalPrice:F2}");
    //    _notificationService.Send(order,
    //        $"Your order for {order.Quantity}x '{book.Title}' has been confirmed. Total: ${order.TotalPrice:F2}");

    //    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    //}

    [HttpPost]
    public async Task<ActionResult<Order>> PlaceOrder(Order order)
    {
        try
        {
            var result = await _orderService.PlaceOrderAsync(order);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = result.Id },
                result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //[HttpGet("{id}")]
    //public async Task<ActionResult<Order>> GetOrder(int id)
    //{
    //    var order = await _context.Orders.FindAsync(id);
    //    if (order == null) return NotFound();
    //    return order;
    //}
    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    //[HttpGet]
    //public async Task<ActionResult<List<Order>>> GetOrders()
    //{
    //    return await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();
    //}
    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetOrders()
    {
        var orders = await _orderService.GetOrdersAsync();
        return Ok(orders);
    }

    //[HttpPatch("{id}/cancel")]
    //public async Task<IActionResult> CancelOrder(int id)
    //{
    //    var order = await _context.Orders.FindAsync(id);
    //    if (order == null) return NotFound();

    //    if (order.Status == "cancelled")
    //        return BadRequest("Order is already cancelled");

    //    var book = await _context.Books.FindAsync(order.BookId);
    //    if (book != null)
    //        book.Stock += order.Quantity;

    //    order.Status = "cancelled";
    //    await _context.SaveChangesAsync();

    //    //SendNotification(order, book,
    //    //    $"Your order #{order.Id} has been cancelled. Refund: ${order.TotalPrice:F2}");
    //    _notificationService.Send(order,
    //        $"Your order #{order.Id} has been cancelled. Refund: ${order.TotalPrice:F2}");

    //    return NoContent();
    //}

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var success = await _orderService.CancelOrderAsync(id);

            if (!success) return NotFound();

            return NoContent();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
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
}
