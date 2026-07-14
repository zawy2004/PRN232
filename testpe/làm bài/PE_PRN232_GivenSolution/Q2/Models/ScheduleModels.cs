namespace Q2.Models
{
    // Item returned by GET /api/schedules/search
    public class ScheduleListItem
    {
        public int SectionId { get; set; }
        public string? InstructorName { get; set; }
        public string? CourseName { get; set; }
        public string? Semester { get; set; }
        public string? RoomNumber { get; set; }
        public int? MaxCapacity { get; set; }
    }

    // Object returned by GET /api/schedules/{sectionId}
    public class ScheduleDetail
    {
        public int SectionId { get; set; }
        public string? CourseName { get; set; }
        public string? RoomNumber { get; set; }
        public List<EnrolledStudent> Students { get; set; } = new();
    }

    public class EnrolledStudent
    {
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public double? Grade { get; set; }
    }
}
