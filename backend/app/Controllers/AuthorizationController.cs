[ApiController]
[Route("authorization")]
public class AuthorizationController(Context context, Config config, UserSession session, JwtTokenService jwt) : ControllerBase
{
    private readonly Config _config = config;
    private readonly UserService _userService = new(context);
    private readonly StudentGroupService _groupService = new(context);
    private readonly EmailService _emailService = new(config);
    private readonly LdapService _ldapService = new(config);

    [HttpGet("check-session")]
    public ActionResult<UserGetDto?> GetUser()
    {
        User user = session.User;
        return Ok(user.MakeGetDto());
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> PostToken(UserLoginDto userDto)
    {
        User? user = await _userService.GetAsync(userDto.Email);

        if (user == null)
        {
            //ad connection
            bool userCanConnect = CheckAdConnection(userDto.Email, userDto.Password);

            if (userCanConnect == false)
            {
                return BadRequest(ResponseMessage.GetWrongCredentials());
            }

            User? adUser = await _ldapService.GetStudentByEmailAsync(userDto.Email);

            if (adUser == null)
            {
                return NotFound(ResponseMessage.GetUserNotFound());
            }

            UserCreateDto userCreate = new UserCreateDto
            {
                Email = adUser.Email.ToLower(),
                Name = adUser.Name,
                Password = userDto.Password,
                Surname = adUser.Surname,
            };

            await CreateUser(userCreate, Models.User.UserRoles.Student);
            user = await _userService.GetAsync(adUser.Email);
        }

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

        if (await _userService.GetAsync(userDto.Email) != null)
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

        if (await _userService.GetAsync(userDto.Email) != null)
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

        if (await _userService.GetAsync(userDto.Email) != null)
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

        if (await _userService.GetAsync(userDto.Email) != null)
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
            Password = Password.GetHash(userDto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        Group? group = null;

        if (userRole == Models.User.UserRoles.Student)
        {
            group = await _groupService.GetByIDAsync(newUser.GroupId ?? -1);
            newUser.GroupId = userDto.GroupId;
        }

        await _userService.CreateAsync(newUser);
        _emailService.SendUserCreation(newUser);
        return newUser.MakeGetDto();
    }

    private bool CheckAdConnection(string email, string password)
    {
        string login = email.Split("@").First();

        LdapConnection? connection = _ldapService.ConnectAsync(login, password);

        if (connection != null && connection.Connected == true)
        {
            return true;
        }

        return false;
    }
}
