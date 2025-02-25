using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using app.Models;

namespace app.Controllers;

public class LoginController(UserSession userSession) : Controller
{
    public IActionResult Index()
    {
        if (userSession.isAuthenticated)
        {
            return Redirect("/");
        }

        return View();
    }

    [HttpPost("/login")]
    public IActionResult Login(string password)
    {
        if (userSession.ChechPassword(password))
        {
            var passwordHash = userSession.GetHash(password);
            Response.Cookies.Append("token", passwordHash);
            return Redirect("/");
        }

        var pageModel = new { 
            Message = "Password is incorrect" 
        };

        return View("Index", pageModel);
    }

    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Redirect("/login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
