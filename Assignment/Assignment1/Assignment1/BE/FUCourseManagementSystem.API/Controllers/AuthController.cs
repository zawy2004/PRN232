using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISystemAccountService _accountService;
    private readonly IConfiguration _config;

    public AuthController(ISystemAccountService accountService, IConfiguration config)
    {
        _accountService = accountService;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var adminEmail = _config["AdminAccount:Email"];
        var adminPassword = _config["AdminAccount:Password"];
        var adminName = _config["AdminAccount:Name"];

        if (request.Email == adminEmail && request.Password == adminPassword)
            return Ok(new { role = "Admin", name = adminName, email = adminEmail, accountId = 0 });

        var account = await _accountService.AuthenticateAsync(request.Email, request.Password);
        if (account == null)
            return Unauthorized(new { message = "Invalid email or password." });

        var roleName = account.AccountRole switch
        {
            1 => "Lecturer",
            2 => "Student",
            3 => "AcademicStaff",
            _ => "Unknown"
        };

        return Ok(new { role = roleName, name = account.AccountName, email = account.AccountEmail, accountId = account.AccountID });
    }
}

public record LoginRequest(string Email, string Password);

