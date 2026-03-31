using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Infrastructure.Notifications
{
    public class SmsNotificationSender : INotificationSender
    {
        public readonly NotificationHub _notificationHub;
        
        public SmsNotificationSender(NotificationHub notificationHub)
        {
            _notificationHub = notificationHub;
        }

        public void Send(Order order, string message)
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
    }
}
