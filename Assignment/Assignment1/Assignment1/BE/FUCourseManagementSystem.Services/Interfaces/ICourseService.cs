using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Services.Interfaces;

public interface ICourseService
{
    IQueryable<Course> GetAll();
    Task<Course?> GetByIdAsync(int id);
    Task CreateAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(int id);
}
