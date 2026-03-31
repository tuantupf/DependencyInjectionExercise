using DependencyInjectionExercise.Infrastructure.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace DependencyInjectionExercise.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationHub _hub;

    public NotificationsController(NotificationHub hub)
    {
        _hub = hub;
    }

    [HttpGet]
    public ActionResult GetAll() => Ok(_hub.GetAll());

    [HttpDelete]
    public ActionResult Clear() { _hub.Clear(); return NoContent(); }
}
