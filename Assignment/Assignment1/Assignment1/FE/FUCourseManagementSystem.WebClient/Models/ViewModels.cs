using System.ComponentModel.DataAnnotations;

namespace FUCourseManagementSystem.WebClient.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class SystemAccountViewModel
{
    public int AccountID { get; set; }
    [Required] public string AccountName { get; set; } = null!;
    [Required, EmailAddress] public string AccountEmail { get; set; } = null!;
    [Required] public int AccountRole { get; set; }
    public string? AccountPassword { get; set; }
    public bool IsActive { get; set; } = true;
}

public class DepartmentViewModel
{
    public int DepartmentID { get; set; }
    [Required] public string DepartmentName { get; set; } = null!;
    [Required] public string DepartmentCode { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CourseViewModel
{
    public int CourseID { get; set; }
    [Required] public string CourseCode { get; set; } = null!;
    [Required] public string CourseName { get; set; } = null!;
    [Range(1, 20)] public int Credits { get; set; }
    public string? Description { get; set; }
    [Required] public int DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsActive { get; set; } = true;
}

public class LecturerViewModel
{
    public int LecturerID { get; set; }
    [Required] public string LecturerName { get; set; } = null!;
    [Required, EmailAddress] public string LecturerEmail { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    [Required] public int DepartmentID { get; set; }
    public string? DepartmentName { get; set; }
    [Required] public int AccountID { get; set; }
    public bool IsActive { get; set; } = true;
}

public class StudentViewModel
{
    public int StudentID { get; set; }
    [Required] public string StudentCode { get; set; } = null!;
    [Required] public string StudentName { get; set; } = null!;
    [Required, EmailAddress] public string StudentEmail { get; set; } = null!;
    public string? Major { get; set; }
    [Required] public int AccountID { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ClassSectionViewModel
{
    public int ClassID { get; set; }
    [Required] public string ClassCode { get; set; } = null!;
    [Required] public int CourseID { get; set; }
    public string? CourseName { get; set; }
    [Required] public int LecturerID { get; set; }
    public string? LecturerName { get; set; }
    [Required] public string Semester { get; set; } = null!;
    public string? Room { get; set; }
    public int? Slot { get; set; }
    [Required] public DateTime StartDate { get; set; }
    [Required] public DateTime EndDate { get; set; }
    [Range(1, 500)] public int MaxStudents { get; set; }
    public bool IsActive { get; set; } = true;
}

public class EnrollmentViewModel
{
    public int EnrollmentID { get; set; }
    public int StudentID { get; set; }
    public string? StudentName { get; set; }
    public int ClassID { get; set; }
    public string? ClassCode { get; set; }
    public string? CourseName { get; set; }
    public DateTime EnrolledDate { get; set; }
    public string EnrollmentStatus { get; set; } = "Pending";
    public string? Note { get; set; }
}
