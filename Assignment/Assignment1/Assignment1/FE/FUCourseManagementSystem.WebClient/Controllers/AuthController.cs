using FUCourseManagementSystem.WebClient.Models;
using FUCourseManagementSystem.WebClient.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace FUCourseManagementSystem.WebClient.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _api;

    public AuthController(ApiService api) => _api = api;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = await _api.PostAsync("api/auth/login", new { model.Email, model.Password });
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);
        var role = data.GetProperty("role").GetString()!;
        var name = data.GetProperty("name").GetString()!;
        var accountId = data.TryGetProperty("accountId", out var accountIdProp)
            ? accountIdProp.GetInt32()
            : 0;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, model.Email),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, accountId.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();
}
