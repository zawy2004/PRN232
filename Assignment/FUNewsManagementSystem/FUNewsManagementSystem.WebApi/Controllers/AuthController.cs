using FUNewsManagementSystem.Repositories.Interfaces;
using FUNewsManagementSystem.WebApi.Models;
using FUNewsManagementSystem.WebApi.Seeding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FUNewsManagementSystem.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ISystemAccountRepository _systemAccountRepository;
    private readonly AdminAccountOptions _adminOptions;

    public AuthController(ISystemAccountRepository systemAccountRepository, IOptions<AdminAccountOptions> adminOptions)
    {
        _systemAccountRepository = systemAccountRepository;
        _adminOptions = adminOptions.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (string.Equals(request.Email, _adminOptions.Email, StringComparison.OrdinalIgnoreCase)
            && request.Password == _adminOptions.Password)
        {
            return Ok(new
            {
                AccountId = 0,
                AccountName = string.IsNullOrWhiteSpace(_adminOptions.Name) ? "Administrator" : _adminOptions.Name,
                AccountEmail = _adminOptions.Email,
                AccountRole = _adminOptions.Role
            });
        }

        var account = await _systemAccountRepository.GetByEmailAndPasswordAsync(request.Email, request.Password);
        if (account is null)
        {
            return Unauthorized();
        }

        return Ok(account);
    }
}
