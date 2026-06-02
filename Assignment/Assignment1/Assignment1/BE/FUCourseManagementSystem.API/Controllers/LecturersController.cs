using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LecturersController : ControllerBase
{
    private readonly ILecturerService _service;

    public LecturersController(ILecturerService service) => _service = service;

    [HttpGet]
    [EnableQuery]
    public IActionResult Get() => Ok(_service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var lecturer = await _service.GetByIdAsync(id);
        return lecturer == null ? NotFound() : Ok(lecturer);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Lecturer lecturer)
    {
        await _service.CreateAsync(lecturer);
        return CreatedAtAction(nameof(Get), new { id = lecturer.LecturerID }, lecturer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Lecturer lecturer)
    {
        lecturer.LecturerID = id;
        try
        {
            await _service.UpdateAsync(lecturer);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
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
    }
}


