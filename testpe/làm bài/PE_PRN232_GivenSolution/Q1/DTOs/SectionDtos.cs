namespace Q1.DTOs
{
    // Used for GET /api/sections (supports JSON and XML)
    public class SectionListItemDto
    {
        public int SectionId { get; set; }
        public string? CourseName { get; set; }
        public string? RoomNumber { get; set; }
        public int? MaxCapacity { get; set; }
        public int EnrolledCount { get; set; }
    }

    // Request body for PUT /api/sections/{sectionId}
    public class UpdateSectionRequest
    {
        public string? RoomNumber { get; set; }
        public int MaxCapacity { get; set; }
    }

    // Request body for POST /api/enrollments
    public class CreateEnrollmentRequest
    {
        public int StudentId { get; set; }
        public int SectionId { get; set; }
    }
}
