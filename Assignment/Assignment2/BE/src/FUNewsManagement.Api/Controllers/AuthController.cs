using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login(LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result is null ? Unauthorized(new { error = "Invalid email or password." }) : Ok(result);
    }

    [HttpPost("google")]
    public async Task<ActionResult<LoginResultDto>> GoogleLogin(GoogleLoginRequestDto dto)
    {
        var result = await _authService.GoogleLoginAsync(dto);
        return result is null
            ? Unauthorized(new { error = "Invalid Google token or no matching account for this email." })
            : Ok(result);
    }
}
