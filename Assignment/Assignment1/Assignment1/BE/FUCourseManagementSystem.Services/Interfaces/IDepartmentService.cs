using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface IDepartmentService
{
    IQueryable<Department> GetAll();
    Task<Department?> GetByIdAsync(int id);
    Task CreateAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(int id);
}
