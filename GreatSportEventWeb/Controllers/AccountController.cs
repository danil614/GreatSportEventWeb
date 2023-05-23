using System.Security.Claims;
using GreatSportEventWeb.Data;
using GreatSportEventWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace GreatSportEventWeb.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationContext _context;

    public AccountController(ApplicationContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult AccessDenied()
    {
        ViewBag.ErrorText = "Для доступа к странице войдите в систему!";
        return View("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Login(User user)
    {
        if (ModelState.IsValid)
        {
            var item = _context.Users.FirstOrDefault(
                item => item.Login == user.Login &&
                        item.Password == HashPassword.GetHash(user.Password));
            
            if (item is null)
            {
                ViewBag.ErrorText = "Пользователь с такими логином и паролем не найден!";
                return View(user);
            }
            
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new(ClaimsIdentity.DefaultRoleClaimType, user.AccessMode.ToString())
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            
            return RedirectToAction("Index", "Home");
        }

        return View(user);
    }
}