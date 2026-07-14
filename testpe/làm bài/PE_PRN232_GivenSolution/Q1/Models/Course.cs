namespace Q1.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; } = null!;
        public int? Credits { get; set; }
        public string? Department { get; set; }

        public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
        public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}
