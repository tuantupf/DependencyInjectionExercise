using DependencyInjectionExercise.Infrastructure.Discounts;
using DependencyInjectionExercise.Infrastructure.Notifications;
using DependencyInjectionExercise.Infrastructure.Repositories;
using DependencyInjectionExercise.Infrastructure.Tracking;
using DependencyInjectionExercise.Models;
using Microsoft.EntityFrameworkCore;

namespace DependencyInjectionExercise.Application
{
    public class OrderService : IOrderService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly DiscountService _discountService;
        private readonly OrderTrackingService _trackingService;
        private readonly NotificationService _notificationService;
        private readonly IServiceProvider _serviceProvider;

        public OrderService(
            IBookRepository bookRepository,
            IOrderRepository orderRepository,
            DiscountService discountService,
            OrderTrackingService trackingService,
            NotificationService notificationService,
            IServiceProvider serviceProvider)
        {
            _bookRepository = bookRepository;
            _orderRepository = orderRepository;
            _discountService = discountService;
            _trackingService = trackingService;
            _notificationService = notificationService;
            _serviceProvider = serviceProvider;
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _orderRepository.GetByIdAsync(id);   
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order> PlaceOrderAsync(Order order)
        {
            var book = await _bookRepository.GetByIdAsync(order.BookId);

            if (book == null)
                throw new Exception("Book not found");

            if (order.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero");

            if (book.Stock < order.Quantity)
                throw new Exception($"Not enough stock. Available: {book.Stock}");

            order.TotalPrice = book.Price * order.Quantity;

            var discountPercent = _discountService.CalculateDiscount(
                book.Category, order.Quantity, order.CustomerName);

            order.DiscountApplied = discountPercent;
            order.TotalPrice *= (1 - discountPercent);

            book.Stock -= order.Quantity;
            order.OrderDate = DateTime.UtcNow;
            order.Status = "confirmed";

            _trackingService.SetTrackingNote($"Order placed for {order.Quantity}x '{book.Title}'");

            await _orderRepository.AddAsync(order);

            var secondTracking = _serviceProvider.GetRequiredService<OrderTrackingService>();
            var trackingNote = secondTracking.GetTrackingNote();
            order.TrackingNote = trackingNote;

            await _bookRepository.SaveChangesAsync();

            _notificationService.Send(order,
                $"Your order for {order.Quantity}x '{book.Title}' has been confirmed. Total: ${order.TotalPrice:F2}");

            return order;
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            if (order.Status == "cancelled")
                throw new Exception("Order is already cancelled");

            var book = await _bookRepository.GetByIdAsync(order.BookId);
            if (book != null)
                book.Stock += order.Quantity;

            order.Status = "cancelled";
            await _orderRepository.SaveChangesAsync();

            _notificationService.Send(order,
                $"Your order #{order.Id} has been cancelled. Refund: ${order.TotalPrice:F2}");

            return true;
        }
    }
}
