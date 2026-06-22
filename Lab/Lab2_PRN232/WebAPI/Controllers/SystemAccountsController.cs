using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAccountsController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountService;

        public SystemAccountsController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] AccountRequestDTO loginDTO)
        {
            try
            {
                var account = await _systemAccountService.Login(loginDTO.Email, loginDTO.Password);
                if (account == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, account.EmailAddress),
                    new Claim("Role", account.Role.ToString()),
                    new Claim("AccountId", account.AccountId.ToString()),
                };

                var symetricKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
                var signCredential = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

                var preparedToken = new JwtSecurityToken(
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(16),
                    signingCredentials: signCredential);

                var generatedToken = new JwtSecurityTokenHandler().WriteToken(preparedToken);
                var role = account.Role.ToString();
                var accountId = account.AccountId.ToString();

                return Ok(new AccountResponseDTO
                {
                    Role = role,
                    Token = generatedToken,
                    AccountId = accountId
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
