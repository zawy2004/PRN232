namespace Q1.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int? StudentID { get; set; }
        public int? SectionID { get; set; }
        public DateOnly? RegistrationDate { get; set; }
        public double? Grade { get; set; }

        public Student? Student { get; set; }
        public ClassSection? Section { get; set; }
    }
}
