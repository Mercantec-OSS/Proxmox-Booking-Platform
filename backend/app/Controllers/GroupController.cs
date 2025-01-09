[ApiController]
[Route("groups")]
public class GroupController(
    GroupRepository groupRepository,
    UserRepository userRepository
    ) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        Group? studentGroup = await groupRepository.GetByIDAsync(id);

        if (studentGroup == null)
            return NotFound(ResponseMessage.GetClassNotFound());

        return Ok(studentGroup.MakeGetDto());
    }

    [HttpGet("all")]
    public async Task<ActionResult> GetAll()
    {
        List<Group> studentGroups = await groupRepository.GetAsync();
        return Ok(studentGroups.ConvertAll(g => g.MakeGetDto()));
    }


    [HttpGet("name/{name}")]
    public async Task<ActionResult> GetByClassName(string name)
    {
        Group? studentGroup = await groupRepository.GetByClassAsync(name);

        if (studentGroup == null)
            return NotFound(ResponseMessage.GetClassNotFound());

        return Ok(studentGroup.MakeGetDto());
    }

    [HttpGet("user-id/{id}")]
    public async Task<ActionResult> GetByUserId(int id)
    {
        User? user = await userRepository.GetAsync(id);

        if (user == null)
            return NotFound(ResponseMessage.GetUserNotFound());

        if (user.GroupId == null)
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("User havn't a class assigned."));

        Group? studentGroup = await groupRepository.GetByIDAsync(user.GroupId ?? 0);

        return Ok(studentGroup?.MakeGetDto());
    }
}
