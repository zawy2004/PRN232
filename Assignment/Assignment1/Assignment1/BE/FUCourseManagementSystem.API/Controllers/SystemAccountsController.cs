using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemAccountsController : ControllerBase
{
    private readonly ISystemAccountService _service;

    public SystemAccountsController(ISystemAccountService service) => _service = service;

    [HttpGet]
    [EnableQuery]
    public IActionResult Get() => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var account = await _service.GetByIdAsync(id);
        return account == null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SystemAccount account)
    {
        try
        {
            await _service.CreateAsync(account);
            return CreatedAtAction(nameof(Get), new { id = account.AccountID }, account);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] SystemAccount account)
    {
        account.AccountID = id;
        try
        {
            await _service.UpdateAsync(account);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}


