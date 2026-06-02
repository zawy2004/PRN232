using System.ComponentModel.DataAnnotations;

namespace FUCourseManagementSystem.BusinessObjects;

public class SystemAccount
{
    [Key]
    public int AccountID { get; set; }

    [Required, MaxLength(100)]
    public string AccountName { get; set; } = null!;

    [Required, MaxLength(150)]
    public string AccountEmail { get; set; } = null!;

    [Required]
    public int AccountRole { get; set; }

    [Required, MaxLength(256)]
    public string AccountPassword { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public Lecturer? Lecturer { get; set; }
    public Student? Student { get; set; }
}
