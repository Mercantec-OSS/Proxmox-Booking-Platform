public class UserSession
{
    private readonly UserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtTokenService _jwtTokenService;
    private User UserFromToken => GetUserFromToken();
    public User User => GetUserFromDb();

    public UserSession(UserRepository userRepository, IHttpContextAccessor httpContextAccessor, JwtTokenService jwtTokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
    }

    private string GetToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return "";
        }

        var token = httpContext.Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return "";
        }

        return token;
    }
    private User GetUserFromToken()
    {
        string token = GetToken();
        User user = _jwtTokenService.GetUser(token) ?? throw new HttpException(HttpStatusCode.Unauthorized, "You are not authenticated");
        return user;
    }

    private User GetUserFromDb()
    {
        User user = _userRepository.GetAsync(UserFromToken.Id).Result ?? throw new HttpException(HttpStatusCode.Unauthorized, "User not found in DB");
        return user;
    }


    public User GetIfAuthenticated()
    {
        return UserFromToken;
    }

    public User GetIfRoles (params User.UserRoles[] roles)
    {
        List<string> rolesList = roles.ToList().ConvertAll(role => role.ToString().ToLower());
        User user = UserFromToken;
        if (!rolesList.Contains(user.Role.ToLower()))
        {
            throw new HttpException(HttpStatusCode.Forbidden, "Insufficient privileges to perform this action");
        }

        return user;
    }

    public User GetIfAdmin()
    {
        return GetIfRoles(User.UserRoles.Admin);
    }

    public User GetIfTeacher()
    {
        return GetIfRoles(User.UserRoles.Teacher);
    }

    public User GetIfStudent()
    {
        return GetIfRoles(User.UserRoles.Student);
    }

    public User GetIfModerator()
    {
        return GetIfRoles(User.UserRoles.Moderator);
    }

    public bool IsAuthenticated()
    {
        return UserFromToken != null;
    }

    public bool IsAdmin()
    {
        return UserFromToken.Role.ToLower() == User.UserRoles.Admin.ToString().ToLower();
    }

    public bool IsTeacher()
    {
        return UserFromToken.Role.ToLower() == User.UserRoles.Teacher.ToString().ToLower();
    }

    public bool IsStudent()
    {
        return UserFromToken.Role.ToLower() == User.UserRoles.Student.ToString().ToLower();
    }

    public bool IsModerator()
    {
        return UserFromToken.Role.ToLower() == User.UserRoles.Moderator.ToString().ToLower();
    }
}