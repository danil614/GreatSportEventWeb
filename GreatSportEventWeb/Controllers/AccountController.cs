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
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        ViewBag.ErrorText = "У вас нет разрешения на доступ к этой странице. " +
                            "Пожалуйста, войдите под другим пользователем, который имеет соответствующие права доступа.";
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
                ViewBag.ErrorText = "Пользователь с указанным логином и паролем не найден!";
                return View(user);
            }

            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, item.Login),
                new(ClaimsIdentity.DefaultRoleClaimType, item.AccessMode.ToString()),
                new(ClaimTypes.GivenName, "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}