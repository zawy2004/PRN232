using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Data;
using Q1.DTOs;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrollmentsController(AppDbContext context)
        {
            _context = context;
        }

        // 4. POST /api/enrollments
        // Add a new enrollment for a student.
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] CreateEnrollmentRequest request)
        {
            // If the StudentId does not exist, return 404 Not Found.
            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
            {
                return NotFound(new { message = "Student not found." });
            }

            // If the SectionId does not exist, return 404 Not Found.
            var section = await _context.ClassSections
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.SectionID == request.SectionId);
            if (section == null)
            {
                return NotFound(new { message = "Section not found." });
            }

            // If the section is already full (EnrolledCount >= MaxCapacity), return 400 Bad Request.
            var enrolledCount = section.Enrollments.Count;
            if (enrolledCount >= (section.MaxCapacity ?? 0))
            {
                return BadRequest(new { message = "Section is full" });
            }

            var enrollment = new Enrollment
            {
                StudentID = request.StudentId,
                SectionID = request.SectionId,
                RegistrationDate = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                enrollmentId = enrollment.EnrollmentID,
                studentId = enrollment.StudentID,
                sectionId = enrollment.SectionID,
                registrationDate = enrollment.RegistrationDate
            });
        }
    }
}
