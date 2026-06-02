using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface IStudentRepository
{
    IQueryable<Student> GetAll();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByAccountIdAsync(int accountId);
    Task AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Student student);
}
