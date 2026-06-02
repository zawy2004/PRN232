using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUCourseManagementSystem.BusinessObjects;

public class ClassSection
{
    [Key]
    public int ClassID { get; set; }

    [Required, MaxLength(50)]
    public string ClassCode { get; set; } = null!;

    public int CourseID { get; set; }

    [ForeignKey(nameof(CourseID))]
    public Course? Course { get; set; }

    public int LecturerID { get; set; }

    [ForeignKey(nameof(LecturerID))]
    public Lecturer? Lecturer { get; set; }

    [Required, MaxLength(20)]
    public string Semester { get; set; } = null!;

    [MaxLength(50)]
    public string? Room { get; set; }

    public int? Slot { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    [Range(1, 500)]
    public int MaxStudents { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
