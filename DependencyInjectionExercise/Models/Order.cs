namespace DependencyInjectionExercise.Models;

public class Order
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DiscountApplied { get; set; }
    public DateTime OrderDate { get; set; }
    public string NotificationMethod { get; set; } = "email"; // "email", "sms", or "push"
    public string Status { get; set; } = "pending";
    public string? TrackingNote { get; set; }
}
