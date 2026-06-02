using FUCourseManagementSystem.BusinessObjects;

namespace FUCourseManagementSystem.Repositories.Interfaces;

public interface ICourseRepository
{
    IQueryable<Course> GetAll();
    Task<Course?> GetByIdAsync(int id);
    Task<bool> HasClassSectionsAsync(int courseId);
    Task AddAsync(Course course);
    Task UpdateAsync(Course course);
    Task DeleteAsync(Course course);
}
