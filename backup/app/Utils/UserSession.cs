using System.Security.Cryptography;
using System.Text;

public class UserSession(IHttpContextAccessor httpContextAccessor)
{
    public bool isAuthenticated => IsAuthenticated();
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }

    public bool ChechPassword(string password)
    {
        var passwordHash = GetHash(password);
        return CheckToken(passwordHash);
    } 

    private bool IsAuthenticated()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return false;
        }

        var token = httpContext.Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        return CheckToken(token);
    }

    private bool CheckToken(string token)
    {
        string passwordHash = GetHash(Config.ACCESS_PASS);
        return passwordHash == token;
    }
}