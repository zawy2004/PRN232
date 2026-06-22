using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service) => _service = service;

    [HttpGet]
    [EnableQuery]
    public IActionResult Get() => Ok(_service.GetAll().Select(c => new
    {
        c.CourseID,
        c.CourseCode,
        c.CourseName,
        c.Credits,
        c.Description,
        c.DepartmentID,
        c.IsActive,
        DepartmentName = c.Department != null ? c.Department.DepartmentName : null
    }));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var course = await _service.GetByIdAsync(id);
        return course == null ? NotFound() : Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Course course)
    {
        await _service.CreateAsync(course);
        return CreatedAtAction(nameof(Get), new { id = course.CourseID }, course);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Course course)
    {
        course.CourseID = id;
        try
        {
            await _service.UpdateAsync(course);
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
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}


