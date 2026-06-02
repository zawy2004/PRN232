using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUCourseManagementSystem.BusinessObjects;

public class Lecturer
{
    [Key]
    public int LecturerID { get; set; }

    [Required, MaxLength(150)]
    public string LecturerName { get; set; } = null!;

    [Required, MaxLength(150)]
    public string LecturerEmail { get; set; } = null!;

    [MaxLength(30)]
    public string? PhoneNumber { get; set; }

    public int DepartmentID { get; set; }

    [ForeignKey(nameof(DepartmentID))]
    public Department? Department { get; set; }

    public int AccountID { get; set; }

    [ForeignKey(nameof(AccountID))]
    public SystemAccount? Account { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
}
