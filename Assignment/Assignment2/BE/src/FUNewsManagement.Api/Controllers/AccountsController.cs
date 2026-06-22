using FUNewsManagement.BusinessLogic.Common;
using FUNewsManagement.BusinessLogic.Dtos;
using FUNewsManagement.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUNewsManagement.Api.Controllers;

[ApiController]
[Route("api/accounts")]
[Authorize(Roles = RoleNames.Admin)]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountDto>>> GetAll() => Ok(await _accountService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDto>> GetById(short id)
    {
        var account = await _accountService.GetByIdAsync(id);
        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(AccountUpsertDto dto)
    {
        var created = await _accountService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.AccountId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(short id, AccountUpsertDto dto)
    {
        await _accountService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(short id)
    {
        await _accountService.DeleteAsync(id);
        return NoContent();
    }
}
