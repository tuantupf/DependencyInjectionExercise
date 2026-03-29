using DependencyInjectionExercise.Models;

namespace DependencyInjectionExercise.Services;

public class NotificationHub
{
    private readonly List<NotificationLog> _logs = [];
    private readonly Lock _lock = new();

    public void Add(NotificationLog log)
    {
        lock (_lock)
        {
            _logs.Add(log);
            // Keep only last 50
            if (_logs.Count > 50)
                _logs.RemoveAt(0);
        }
    }

    public List<NotificationLog> GetAll()
    {
        lock (_lock)
        {
            return [.. _logs];
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _logs.Clear();
        }
    }
}
