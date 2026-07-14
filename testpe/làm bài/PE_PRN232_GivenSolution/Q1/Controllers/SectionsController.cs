using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Data;
using Q1.DTOs;

namespace Q1.Controllers
{
    [Route("api/sections")]
    [ApiController]
    public class SectionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SectionsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET /api/sections
        // Retrieve a list of all class sections with occupancy status.
        // Supports both JSON and XML (via media formatters configured in Program.cs).
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SectionListItemDto>>> GetSections()
        {
            var result = await _context.ClassSections
                .Select(s => new SectionListItemDto
                {
                    SectionId = s.SectionID,
                    CourseName = s.Course != null ? s.Course.CourseName : null,
                    RoomNumber = s.RoomNumber,
                    MaxCapacity = s.MaxCapacity,
                    EnrolledCount = s.Enrollments.Count()
                })
                .ToListAsync();

            return Ok(result);
        }

        // 2. GET /api/sections/search
        // Retrieve a filtered and paginated list of class sections.
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? semester,
            [FromQuery] bool? isFull,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            // Error handling: 400 Bad Request if page or pageSize is invalid.
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { message = "Page and pageSize must be greater than 0." });
            }

            var entityQuery = _context.ClassSections.AsQueryable();

            // Filtering: by semester name.
            if (!string.IsNullOrEmpty(semester))
            {
                entityQuery = entityQuery.Where(s => s.Semester != null && s.Semester.Contains(semester));
            }

            var projected = entityQuery.Select(s => new SectionListItemDto
            {
                SectionId = s.SectionID,
                CourseName = s.Course != null ? s.Course.CourseName : null,
                RoomNumber = s.RoomNumber,
                MaxCapacity = s.MaxCapacity,
                EnrolledCount = s.Enrollments.Count()
            });

            // isFull filter:
            //  - true  -> only sections where EnrolledCount >= MaxCapacity
            //  - false -> only sections with remaining seats
            if (isFull.HasValue)
            {
                if (isFull.Value)
                    projected = projected.Where(s => s.EnrolledCount >= s.MaxCapacity);
                else
                    projected = projected.Where(s => s.EnrolledCount < s.MaxCapacity);
            }

            var totalSections = await projected.CountAsync();
            var totalPages = (int)Math.Ceiling(totalSections / (double)pageSize);

            var data = await projected
                .OrderBy(s => s.SectionId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                data,
                totalSections,
                totalPages,
                currentPage = page,
                pageSize
            });
        }

        // 3. PUT /api/sections/{sectionId}
        // Update the information of an existing class section (RoomNumber and MaxCapacity).
        [HttpPut("{sectionId}")]
        public async Task<IActionResult> UpdateSection(int sectionId, [FromBody] UpdateSectionRequest request)
        {
            var section = await _context.ClassSections
                .Include(s => s.Course)
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.SectionID == sectionId);

            // If the SectionId does not exist, return 404 Not Found.
            if (section == null)
            {
                return NotFound(new { message = "Section not found." });
            }

            var enrolledCount = section.Enrollments.Count;

            // If the new MaxCapacity is less than the current EnrolledCount, return 400 Bad Request.
            if (request.MaxCapacity < enrolledCount)
            {
                return BadRequest(new
                {
                    message = "New capacity cannot be lower than the current number of enrolled students",
                    currentEnrolled = enrolledCount,
                    requestedCapacity = request.MaxCapacity
                });
            }

            section.RoomNumber = request.RoomNumber;
            section.MaxCapacity = request.MaxCapacity;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Section updated successfully",
                sectionId = section.SectionID,
                courseId = section.CourseID,
                courseName = section.Course?.CourseName,
                roomNumber = section.RoomNumber,
                maxCapacity = section.MaxCapacity,
                enrolledCount,
                availableSeats = (section.MaxCapacity ?? 0) - enrolledCount
            });
        }
    }
}
