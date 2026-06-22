using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassSectionsController : ControllerBase
{
    private readonly IClassSectionService _service;

    public ClassSectionsController(IClassSectionService service) => _service = service;

    [HttpGet]
    [EnableQuery]
    public IActionResult Get() => Ok(_service.GetAll().Select(cs => new
    {
        cs.ClassID,
        cs.ClassCode,
        cs.CourseID,
        cs.LecturerID,
        cs.Semester,
        cs.Room,
        cs.Slot,
        cs.StartDate,
        cs.EndDate,
        cs.MaxStudents,
        cs.IsActive,
        CourseName = cs.Course != null ? cs.Course.CourseName : null,
        LecturerName = cs.Lecturer != null ? cs.Lecturer.LecturerName : null
    }));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var section = await _service.GetByIdAsync(id);
        return section == null ? NotFound() : Ok(section);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ClassSection classSection)
    {
        try
        {
            await _service.CreateAsync(classSection);
            return CreatedAtAction(nameof(Get), new { id = classSection.ClassID }, classSection);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ClassSection classSection)
    {
        classSection.ClassID = id;
        try
        {
            await _service.UpdateAsync(classSection);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
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


