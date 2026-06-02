using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    IQueryable<Enrollment> GetAll();
    Task<Enrollment?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int studentId, int classId);
        Task<Enrollment?> GetByStudentAndClassAsync(int studentId, int classId);
    Task AddAsync(Enrollment enrollment);
    Task UpdateAsync(Enrollment enrollment);
    Task DeleteAsync(Enrollment enrollment);
}
