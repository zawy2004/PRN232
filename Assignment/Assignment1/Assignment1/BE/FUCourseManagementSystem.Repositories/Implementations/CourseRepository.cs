using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.DataAccess;
using FUCourseManagementSystem.DataAccess.DAOs;
using FUCourseManagementSystem.Repositories.Interfaces;

namespace FUCourseManagementSystem.Repositories.Implementations;

public class CourseRepository : ICourseRepository
{
    private readonly CourseDAO _dao;

    public CourseRepository(AppDbContext context)
        => _dao = new CourseDAO(context);

    public IQueryable<Course> GetAll() => _dao.GetAll();
    public Task<Course?> GetByIdAsync(int id) => _dao.GetByIdAsync(id);
    public Task<bool> HasClassSectionsAsync(int courseId) => _dao.HasClassSectionsAsync(courseId);
    public Task AddAsync(Course course) => _dao.AddAsync(course);
    public Task UpdateAsync(Course course) => _dao.UpdateAsync(course);
    public Task DeleteAsync(Course course) => _dao.DeleteAsync(course);
}
