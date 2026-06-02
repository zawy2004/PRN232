using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FUCourseManagementSystem.BusinessObjects;

public class Course
{
    [Key]
    public int CourseID { get; set; }

    [Required, MaxLength(30)]
    public string CourseCode { get; set; } = null!;

    [Required, MaxLength(200)]
    public string CourseName { get; set; } = null!;

    [Range(1, 20)]
    public int Credits { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int DepartmentID { get; set; }

    [ForeignKey(nameof(DepartmentID))]
    public Department? Department { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
}
