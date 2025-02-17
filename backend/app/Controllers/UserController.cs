[ApiController]
[Route("users")]
public class UserController(
    UserSession session,
    EmailService emailService,
    UserRepository userRepository
    ) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserGetDto>> GetUser(int id)
    {
        session.GetIfAuthenticated();

        User? user = await userRepository.GetAsync(id);

        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        return Ok(user.MakeGetDto());
    }

    [HttpGet("all")]
    public async Task<ActionResult<UserGetDto>> GetAllUsers()
    {
        session.GetIfAuthenticated();

        var allUsers = await userRepository.GetAllAsync();
        return Ok(allUsers.ConvertAll(u => u.MakeGetDto()));
    }

    [HttpGet("role/{id}")]
    public async Task<ActionResult<string>> GetUserRole(int id)
    {
        session.GetIfAuthenticated();

        User? user = await userRepository.GetAsync(id);
        
        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        return Ok(user.Role);
    }

    // [HttpGet("class/{id}")]
    // public async Task<ActionResult> GetUsersByClassId(int id)
    // {
    //     session.GetIfAuthenticated();

    //     Group? group = await groupRepository.GetByIDAsync(id);
        
    //     if (group == null)
    //     {
    //         return NotFound(ResponseMessage.GetClassNotFound());
    //     }

    //     var classUsers = await userRepository.GetByClassAsync(id);
    //     return Ok(group.Members.ConvertAll(u => u.MakeGetDto()));
    // }

    // [HttpGet("class/name/{className}")]
    // public async Task<ActionResult> GetUsersByClassName(string className)
    // {
    //     session.GetIfAuthenticated();

    //     Group? group = await groupRepository.GetByClassAsync(className);
        
    //     if (group == null)
    //     {
    //         return NotFound(ResponseMessage.GetClassNotFound());
    //     }

    //     var classUsers = await userRepository.GetByClassAsync(group.Id);
    //     return Ok(group.Members.ConvertAll(u => u.MakeGetDto()));
    // }

    [HttpPut]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> PutUser(UserGetDto userDTO)
    {
        session.GetIfAuthenticated();

        User? user = await userRepository.GetAsync(userDTO.Id);
        
        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        User UserInfo = new User
        {
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
            UpdatedAt = DateTime.UtcNow
        };

        user.Name = userDTO.Name;
        user.Surname = userDTO.Surname;
        user.Email = userDTO.Email;
        user.UpdatedAt = DateTime.UtcNow;

        EmailDto email = EmailDto.GetUserUpdate(UserInfo, user);
        await emailService.SendAsync(email);

        await userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPut("/update/role")]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> UpdateUserRole(UserGetDto userDTO)
    {
        session.GetIfAuthenticated();

        User? user = await userRepository.GetAsync(userDTO.Id);

        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        if (
            userDTO.Role.ToLower() != Models.User.UserRoles.Student.ToString().ToLower() &&
            userDTO.Role.ToLower() != Models.User.UserRoles.Moderator.ToString().ToLower() &&
            userDTO.Role.ToLower() != Models.User.UserRoles.Teacher.ToString().ToLower() &&
            userDTO.Role.ToLower() != Models.User.UserRoles.Admin.ToString().ToLower()
        )
        {
            return BadRequest(ResponseMessage.GetErrorMessage("User has an invalid role."));
        }

        User previousRole = new User
        {
            Role = user.Role,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
        };

        user.Role = userDTO.Role;

        EmailDto email = EmailDto.GetUserRoleUpdate(user, previousRole.Role);
        await emailService.SendAsync(email);

        await userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpPut("/update/class")]
    [ProducesResponseType(204)]
    public async Task<ActionResult> UpdateUserClass(int userId, int? classId = null)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        User? user = await userRepository.GetAsync(userId);

        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        if (user.Role.ToLower() == Models.User.UserRoles.Student.ToString().ToLower())
        {
            return BadRequest(ResponseMessage.GetErrorMessage("Only students can get assigned to a class."));
        }

        await userRepository.UpdateAsync(user);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    public async Task<ActionResult<bool>> DeleteUser(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin
        );

        User? user = await userRepository.GetAsync(id);

        if (user == null)
        {
            return NotFound(ResponseMessage.GetUserNotFound());
        }

        await userRepository.DeleteAsync(user);
        return NoContent();
    }
}
