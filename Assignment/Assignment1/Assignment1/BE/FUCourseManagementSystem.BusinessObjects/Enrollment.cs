using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUCourseManagementSystem.BusinessObjects;

public enum EnrollmentStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3
}

public class Enrollment
{
    [Key]
    public int EnrollmentID { get; set; }

    public int StudentID { get; set; }

    [ForeignKey(nameof(StudentID))]
    public Student? Student { get; set; }

    public int ClassID { get; set; }

    [ForeignKey(nameof(ClassID))]
    public ClassSection? ClassSection { get; set; }

    public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;

    [Required]
    public EnrollmentStatus EnrollmentStatus { get; set; } = EnrollmentStatus.Pending;

    [MaxLength(500)]
    public string? Note { get; set; }
}
