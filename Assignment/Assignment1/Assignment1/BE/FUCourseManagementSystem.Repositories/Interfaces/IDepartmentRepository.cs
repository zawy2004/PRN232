using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface IDepartmentRepository
{
    IQueryable<Department> GetAll();
    Task<Department?> GetByIdAsync(int id);
    Task<bool> HasCoursesAsync(int departmentId);
    Task AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(Department department);
}
