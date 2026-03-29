namespace DependencyInjectionExercise.Services;

public class OrderTrackingService
{
    private readonly string _instanceId = Guid.NewGuid().ToString()[..8];
    private string? _trackingNote;

    public string InstanceId => _instanceId;

    public void SetTrackingNote(string note)
    {
        _trackingNote = $"[{_instanceId}] {note}";
        Console.WriteLine($"OrderTrackingService ({_instanceId}): Set note -> {_trackingNote}");
    }

    public string? GetTrackingNote()
    {
        Console.WriteLine($"OrderTrackingService ({_instanceId}): Get note -> {_trackingNote ?? "NULL"}");
        return _trackingNote;
    }
}
