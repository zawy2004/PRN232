namespace Q1.Models
{
    public class CourseAssignment
    {
        public int CourseID { get; set; }
        public int InstructorID { get; set; }
        public DateOnly? AssignmentDate { get; set; }

        public Course Course { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;
    }
}
