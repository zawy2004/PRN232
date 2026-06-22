using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service) => _service = service;

    [HttpGet]
    [EnableQuery]
    public IActionResult Get() => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var dept = await _service.GetByIdAsync(id);
        return dept == null ? NotFound() : Ok(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Department department)
    {
        await _service.CreateAsync(department);
        return CreatedAtAction(nameof(Get), new { id = department.DepartmentID }, department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Department department)
    {
        department.DepartmentID = id;
        try
        {
            await _service.UpdateAsync(department);
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
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}


