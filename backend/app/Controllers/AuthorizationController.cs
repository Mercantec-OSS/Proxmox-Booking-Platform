[ApiController]
[Route("authorization")]
public class AuthorizationController(
    UserRepository userRepository,
    UserSession session, 
    JwtTokenService jwt,
    EmailService emailService
    ) : ControllerBase
{
    [HttpGet("check-session")]
    public ActionResult<UserGetDto?> GetUser()
    {
        User user = session.User;
        return Ok(user.MakeGetDto());
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> PostToken(UserLoginDto userDto)
    {
        User? user = await userRepository.GetAsync(userDto.Email);

        if (user == null)
        {
            return Unauthorized(ResponseMessage.GetErrorMessage("Intern error. User not found."));
        }

        if (user.Password == null)
        {
            return Unauthorized(ResponseMessage.GetErrorMessage("Intern error. User password not found."));
        }

        if (!Password.Verify(userDto.Password, user.Password))
        {
            return Unauthorized(ResponseMessage.GetWrongCredentials());
        }

        string token = jwt.CreateToken(user);

        HttpContext.Response.Cookies.Append("token", token);
        return Ok(new TokenDto { Token = token });
    }

    [HttpDelete("logout")]
    [ProducesResponseType(204)]
    public ActionResult<bool> DeleteToken()
    {
        HttpContext.Response.Cookies.Delete("token");
        return NoContent();
    }

    [HttpPost("create/student")]
    [ProducesResponseType(201)]
    public async Task<ActionResult<UserGetDto>> PostCreateStudent(UserCreateDto userDto)
    {
        if (userDto == null)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("User dto not valid."));
        }

        if (await userRepository.GetAsync(userDto.Email) != null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("User need Unique Email."));
        }

        userDto.GroupId = null; //NEED TO REMOWE
        return Ok(await CreateUser(userDto, Models.User.UserRoles.Student));
    }

    [HttpPost("create/teacher")]
    [ProducesResponseType(201)]
    public async Task<ActionResult<UserGetDto>> PostCreateTeacher(UserCreateDto userDto)
    {
        session.GetIfRoles(Models.User.UserRoles.Admin);

        if (userDto == null)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("User dto not valid."));
        }

        if (await userRepository.GetAsync(userDto.Email) != null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("User need Unique Email."));
        }

        return Ok(await CreateUser(userDto, Models.User.UserRoles.Teacher));
    }

    [HttpPost("create/administrator")]
    [ProducesResponseType(201)]
    public async Task<ActionResult<UserGetDto>> PostCreateAdmin(UserCreateDto userDto)
    {
        session.GetIfRoles(Models.User.UserRoles.Admin);

        if (userDto == null)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("User dto not valid."));
        }

        if (await userRepository.GetAsync(userDto.Email) != null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("User need Unique Email."));
        }

        return Ok(await CreateUser(userDto, Models.User.UserRoles.Admin));
    }

    [HttpPost("create/moderator")]
    [ProducesResponseType(201)]
    public async Task<ActionResult<UserGetDto>> PostCreateModerator(UserCreateDto userDto)
    {
        session.GetIfRoles(Models.User.UserRoles.Admin);

        if (userDto == null)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("User dto not valid."));
        }

        if (await userRepository.GetAsync(userDto.Email) != null)
        {
            return UnprocessableEntity(ResponseMessage.GetErrorMessage("User need Unique Email."));
        }

        return Ok(await CreateUser(userDto, Models.User.UserRoles.Moderator));
    }

    private async Task<UserGetDto> CreateUser(UserCreateDto userDto, User.UserRoles userRole)
    {
        User newUser = new()
        {
            Id = 0,
            Name = userDto.Name,
            Surname = userDto.Surname,
            Email = userDto.Email.ToLower(),
            Role = userRole.ToString(),
            GroupId = userDto.GroupId,
            Password = Password.GetHash(userDto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await userRepository.CreateAsync(newUser);

        Email email = Email.GetUserCreation(newUser);
        await emailService.SendAsync(email);

        return newUser.MakeGetDto();
    }
}
