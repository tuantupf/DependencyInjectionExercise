using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Infrastructure.Notifications
{
    public interface INotificationSender
    {
        void Send(Order order, string message);
    }
}
