using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface IStudentService
{
    IQueryable<Student> GetAll();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByAccountIdAsync(int accountId);
    Task CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int id);
}
