namespace DependencyInjectionExercise.Models;

public class NotificationLog
{
    public DateTime Timestamp { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
