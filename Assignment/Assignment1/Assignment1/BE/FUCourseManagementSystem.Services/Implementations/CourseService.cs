using FUCourseManagementSystem.BusinessObjects;
using FUCourseManagementSystem.Repositories.Interfaces;
using FUCourseManagementSystem.Services.Interfaces;

namespace FUCourseManagementSystem.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _repo;

    public CourseService(ICourseRepository repo) => _repo = repo;

    public IQueryable<Course> GetAll() => _repo.GetAll();

    public Task<Course?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

    public Task CreateAsync(Course course) => _repo.AddAsync(course);

    public async Task UpdateAsync(Course course)
    {
        var existing = await _repo.GetByIdAsync(course.CourseID)
            ?? throw new KeyNotFoundException("Course not found.");
        existing.CourseCode = course.CourseCode;
        existing.CourseName = course.CourseName;
        existing.Credits = course.Credits;
        existing.Description = course.Description;
        existing.DepartmentID = course.DepartmentID;
        existing.IsActive = course.IsActive;
        await _repo.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Course not found.");
        if (await _repo.HasClassSectionsAsync(id))
            throw new InvalidOperationException("Cannot delete course that has class sections.");
        await _repo.DeleteAsync(course);
    }
}
