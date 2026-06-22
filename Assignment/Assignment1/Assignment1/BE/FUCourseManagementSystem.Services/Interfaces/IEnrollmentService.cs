using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface IEnrollmentService
{
    IQueryable<Enrollment> GetAll();
    Task<Enrollment?> GetByIdAsync(int id);
        Task<Enrollment?> GetByStudentAndClassAsync(int studentId, int classId);
    Task EnrollAsync(int studentId, int classId);
    Task UpdateStatusAsync(int enrollmentId, EnrollmentStatus status, string? note = null);
    Task CancelAsync(int enrollmentId, int studentId);
    Task DeleteAsync(int id);
}
