using System.ComponentModel.DataAnnotations;

namespace FUCourseManagementSystem.BusinessObjects;

public class Department
{
    [Key]
    public int DepartmentID { get; set; }

    [Required, MaxLength(150)]
    public string DepartmentName { get; set; } = null!;

    [Required, MaxLength(20)]
    public string DepartmentCode { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
}
