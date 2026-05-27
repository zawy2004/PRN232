using FUNewsManagementSystem.DataAccess.Entities;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace FUNewsManagementSystem.WebApi.Controllers.OData;

[ApiController]
[Route("odata")]
public class SystemAccountsController : ODataController
{
    private readonly ISystemAccountRepository _repository;

    public SystemAccountsController(ISystemAccountRepository repository)
    {
        _repository = repository;
    }

    [EnableQuery]
    [HttpGet("SystemAccounts")]
    public async Task<IActionResult> Get()
    {
        var accounts = await _repository.GetAllAsync();
        return Ok(accounts);
    }

    [EnableQuery]
    [HttpGet("SystemAccounts({key})")]
    public async Task<IActionResult> Get([FromRoute] int key)
    {
        var account = await _repository.GetByIdAsync(key);
        if (account is null)
        {
            return NotFound();
        }

        return Ok(account);
    }

    [HttpPost("SystemAccounts")]
    public async Task<IActionResult> Post([FromBody] SystemAccount account)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _repository.AddAsync(account);
        return Created(created);
    }

    [HttpPut("SystemAccounts({key})")]
    public async Task<IActionResult> Put([FromRoute] int key, [FromBody] SystemAccount account)
    {
        if (key != account.AccountId)
        {
            return BadRequest("AccountId does not match key.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _repository.UpdateAsync(account);
        if (!updated)
        {
            return NotFound();
        }

        return Updated(account);
    }

    [HttpDelete("SystemAccounts({key})")]
    public async Task<IActionResult> Delete([FromRoute] int key)
    {
        var canDelete = await _repository.CanDeleteAsync(key);
        if (!canDelete)
        {
            return BadRequest("Cannot delete account because it has created news articles.");
        }

        var deleted = await _repository.DeleteAsync(key);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
