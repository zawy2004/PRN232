namespace Q1.Models
{
    public class Instructor
    {
        public int InstructorID { get; set; }
        public string FullName { get; set; } = null!;
        public string? Expertise { get; set; }
        public DateOnly? HireDate { get; set; }

        public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}
