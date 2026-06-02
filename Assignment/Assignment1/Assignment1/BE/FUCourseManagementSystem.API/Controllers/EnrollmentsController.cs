using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;

namespace FUCourseManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _service;
    private readonly IStudentService _studentService;

    public EnrollmentsController(IEnrollmentService service, IStudentService studentService)
    {
        _service = service;
        _studentService = studentService;
    }

    [HttpGet]
    [EnableQuery]
    public IActionResult Get() => Ok(_service.GetAll().Select(e => new
    {
        e.EnrollmentID,
        e.StudentID,
        e.ClassID,
        e.EnrolledDate,
        e.Note,
        EnrollmentStatus = e.EnrollmentStatus.ToString(),
        StudentName = e.Student != null ? e.Student.StudentName : null,
        ClassCode = e.ClassSection != null ? e.ClassSection.ClassCode : null,
        CourseName = e.ClassSection != null && e.ClassSection.Course != null
            ? e.ClassSection.Course.CourseName : null
    }));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var enrollment = await _service.GetByIdAsync(id);
        return enrollment == null ? NotFound() : Ok(enrollment);
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
        try
        {
            await _service.EnrollAsync(request.StudentId, request.ClassId);
            return Ok(new { message = "Enrolled successfully." });
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            await _service.UpdateStatusAsync(id, request.Status, request.Note);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelRequest request)
    {
        if (request == null || request.AccountId <= 0)
            return Unauthorized(new { message = "Invalid account." });

        try
        {
            var enrollment = await _service.GetByIdAsync(id);
            if (enrollment == null) return NotFound();

            var student = await _studentService.GetByAccountIdAsync(request.AccountId);
            if (student == null) return Unauthorized(new { message = "Student profile not found." });

            await _service.CancelAsync(id, student.StudentID);
            return NoContent();
        }
        catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
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

public record EnrollRequest(int StudentId, int ClassId);
public record UpdateStatusRequest(EnrollmentStatus Status, string? Note);
public record CancelRequest(int AccountId);


