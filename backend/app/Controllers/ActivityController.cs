[ApiController]
[Route("activity")]
public class ActivityController(ActivityService activityService) : ControllerBase
{
    private readonly ActivityService _activityService = activityService;

    [HttpGet("all")]
    public IActionResult GetAll()
    {
        return Ok(_activityService.GetAll());
    }

    [HttpGet("user/{id}")]
    public IActionResult GetByUser(int id)
    {
        return Ok(_activityService.GetByUserId(id));
    }

    [HttpPost("")]
    [ProducesResponseType(201)]
    public IActionResult TestCreate(int bookingId, string type)
    {
        if (!Enum.TryParse(type, out Activity.ActivityType activityType))
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("Unknown activity type. Allowed values: " + string.Join(", ", Enum.GetNames(typeof(Activity.ActivityType)))));

        if (HttpContext.Items["user"] is not User user) 
            return NotFound(ResponseMessage.GetUserNotFound());
        
        Activity activity = _activityService.CreateActivity(user.Id, bookingId, Activity.ActivityEvent.Create, activityType);
        return Ok(activity);
    }

    [HttpDelete("all")]
    [ProducesResponseType(204)]
    public IActionResult DeleteAll()
    {
        _activityService.DeleteAll();
        return NoContent();
    }
}