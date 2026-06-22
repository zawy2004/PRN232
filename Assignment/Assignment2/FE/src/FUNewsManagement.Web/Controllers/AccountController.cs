using System.Security.Claims;
using FUNewsManagement.Web.Common;
using FUNewsManagement.Web.Models.Dtos;
using FUNewsManagement.Web.Services.ApiClients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAuthApiClient _authApiClient;
    private readonly IAccountApiClient _accountApiClient;
    private readonly IConfiguration _configuration;

    public AccountController(IAuthApiClient authApiClient, IAccountApiClient accountApiClient, IConfiguration configuration)
    {
        _authApiClient = authApiClient;
        _accountApiClient = accountApiClient;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null, bool sessionExpired = false)
    {
        ViewBag.GoogleClientId = _configuration["Google:ClientId"];
        ViewBag.ReturnUrl = returnUrl;
        if (sessionExpired)
        {
            TempData["LoginError"] = "Your session has expired. Please log in again.";
        }
        return View(new LoginRequestDto());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto model, string? returnUrl = null)
    {
        ViewBag.GoogleClientId = _configuration["Google:ClientId"];
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid) return View(model);

        var result = await _authApiClient.LoginAsync(model.Email, model.Password);
        if (result is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        await SignInAsync(result);
        return RedirectToLocal(returnUrl);
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GoogleLogin(string credential, string? returnUrl = null)
    {
        var result = await _authApiClient.GoogleLoginAsync(credential);
        if (result is null)
        {
            TempData["LoginError"] = "Google sign-in failed: no matching account for this email.";
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        await SignInAsync(result);
        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // ---- Admin: user management ----

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var accounts = await _accountApiClient.GetAllAsync();
        return View(accounts);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AccountUpsertDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _accountApiClient.CreateAsync(model);
            }
            catch (ApiException ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(short id, AccountUpsertDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _accountApiClient.UpdateAsync(id, model);
            }
            catch (ApiException ex)
            {
                TempData["Error"] = ex.Message;
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(short id)
    {
        try
        {
            await _accountApiClient.DeleteAsync(id);
        }
        catch (ApiException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task SignInAsync(LoginResultDto result)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.AccountId.ToString()),
            new(ClaimTypes.Name, result.AccountName),
            new(ClaimTypes.Email, result.Email),
            new(ClaimTypes.Role, result.Role),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }

    private IActionResult RedirectToLocal(string? returnUrl) =>
        !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
}
