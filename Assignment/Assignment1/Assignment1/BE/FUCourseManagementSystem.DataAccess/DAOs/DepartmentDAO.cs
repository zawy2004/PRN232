using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class DepartmentDAO
{
    private static DepartmentDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public DepartmentDAO(AppDbContext context) => _context = context;

    public static DepartmentDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new DepartmentDAO(context);
            return _instance;
        }
    }

    public IQueryable<Department> GetAll() => _context.Departments.AsNoTracking();

    public async Task<Department?> GetByIdAsync(int id)
        => await _context.Departments.FindAsync(id);

    public async Task<bool> HasCoursesAsync(int departmentId)
        => await _context.Courses.AnyAsync(c => c.DepartmentID == departmentId);

    public async Task AddAsync(Department department)
    {
        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Department department)
    {
        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
    }
}
