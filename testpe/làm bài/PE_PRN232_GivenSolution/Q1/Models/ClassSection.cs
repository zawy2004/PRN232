namespace Q1.Models
{
    public class ClassSection
    {
        public int SectionID { get; set; }
        public int? CourseID { get; set; }
        public string? RoomNumber { get; set; }
        public string? Semester { get; set; }
        public int? MaxCapacity { get; set; }

        public Course? Course { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
