namespace Q1.Models
{
    public class Student
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; } = null!;
        public string? Email { get; set; }
        public DateOnly? DateOfBirth { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
