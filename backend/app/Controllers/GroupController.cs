[ApiController]
[Route("groups")]
public class GroupController(Context context) : ControllerBase
{
    private readonly StudentGroupService _studentGroupService = new(context);
    private readonly UserService _userService = new(context);

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        Group? studentGroup = await _studentGroupService.GetByIDAsync(id);

        if (studentGroup == null)
            return NotFound(ResponseMessage.GetClassNotFound());

        return Ok(studentGroup.MakeGetDto());
    }

    [HttpGet("all")]
    public async Task<ActionResult> GetAll()
    {
        List<Group> studentGroups = await _studentGroupService.GetAsync();
        return Ok(studentGroups.ConvertAll(g => g.MakeGetDto()));
    }


    [HttpGet("name/{name}")]
    public async Task<ActionResult> GetByClassName(string name)
    {
        Group? studentGroup = await _studentGroupService.GetByClassAsync(name);

        if (studentGroup == null)
            return NotFound(ResponseMessage.GetClassNotFound());

        return Ok(studentGroup.MakeGetDto());
    }

    [HttpGet("user-id/{id}")]
    public async Task<ActionResult> GetByUserId(int id)
    {
        User? user = await _userService.GetAsync(id);

        if (user == null)
            return NotFound(ResponseMessage.GetUserNotFound());

        if (user.GroupId == null)
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("User havn't a class assigned."));

        Group? studentGroup = await _studentGroupService.GetByIDAsync(user.GroupId ?? 0);

        return Ok(studentGroup?.MakeGetDto());
    }
}
