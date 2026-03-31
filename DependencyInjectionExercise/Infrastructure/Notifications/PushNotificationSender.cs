using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Infrastructure.Notifications
{
    public class PushNotificationSender : INotificationSender
    {
        private readonly NotificationHub _notificationHub;

        public PushNotificationSender(NotificationHub notificationHub)
        {
            _notificationHub = notificationHub;
        }
        public void Send(Order order, string message)
        {
            Console.WriteLine($"[PUSH] To: {order.CustomerName} | {message}");

            _notificationHub.Add(new NotificationLog
            {
                Timestamp = DateTime.UtcNow,
                Channel = "push",
                Recipient = order.CustomerName,
                Message = message
            });
        }
    }
}
