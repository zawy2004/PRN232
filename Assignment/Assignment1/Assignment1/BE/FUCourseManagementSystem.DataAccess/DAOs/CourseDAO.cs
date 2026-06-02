using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class CourseDAO
{
    private static CourseDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public CourseDAO(AppDbContext context) => _context = context;

    public static CourseDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new CourseDAO(context);
            return _instance;
        }
    }

    public IQueryable<Course> GetAll()
        => _context.Courses.AsNoTracking();

    public async Task<Course?> GetByIdAsync(int id)
        => await _context.Courses.Include(c => c.Department).FirstOrDefaultAsync(c => c.CourseID == id);

    public async Task<bool> HasClassSectionsAsync(int courseId)
        => await _context.ClassSections.AnyAsync(cs => cs.CourseID == courseId);

    public async Task AddAsync(Course course)
    {
        await _context.Courses.AddAsync(course);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Course course)
    {
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Course course)
    {
        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
    }
}
