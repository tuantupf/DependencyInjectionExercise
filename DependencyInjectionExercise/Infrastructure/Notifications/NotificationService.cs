using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Infrastructure.Notifications
{
    public class NotificationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Send(Order order, string message)
        {
            var key = order.NotificationMethod.ToLower();

            var sender = _serviceProvider
                .GetRequiredKeyedService<INotificationSender>(key);
            if (sender == null)
            {
                Console.WriteLine($"[UNKNOWN CHANNEL: {order.NotificationMethod}] {message}");
            }
            else
            {
                sender.Send(order, message);
            }
        }
    }
}
