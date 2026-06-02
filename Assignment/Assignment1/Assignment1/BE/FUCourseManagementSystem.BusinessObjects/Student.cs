using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUCourseManagementSystem.BusinessObjects;

public class Student
{
    [Key]
    public int StudentID { get; set; }

    [Required, MaxLength(30)]
    public string StudentCode { get; set; } = null!;

    [Required, MaxLength(150)]
    public string StudentName { get; set; } = null!;

    [Required, MaxLength(150)]
    public string StudentEmail { get; set; } = null!;

    [MaxLength(100)]
    public string? Major { get; set; }

    public int AccountID { get; set; }

    [ForeignKey(nameof(AccountID))]
    public SystemAccount? Account { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
