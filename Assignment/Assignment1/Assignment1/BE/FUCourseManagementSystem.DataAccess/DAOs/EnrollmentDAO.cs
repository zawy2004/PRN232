using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class EnrollmentDAO
{
    private static EnrollmentDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public EnrollmentDAO(AppDbContext context) => _context = context;

    public static EnrollmentDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new EnrollmentDAO(context);
            return _instance;
        }
    }

    public IQueryable<Enrollment> GetAll()
        => _context.Enrollments.AsNoTracking();

    public async Task<Enrollment?> GetByIdAsync(int id)
        => await _context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.ClassSection).ThenInclude(cs => cs!.Course)
            .FirstOrDefaultAsync(e => e.EnrollmentID == id);

    public async Task<bool> ExistsAsync(int studentId, int classId)
        => await _context.Enrollments.AnyAsync(e => e.StudentID == studentId && e.ClassID == classId);

    public async Task AddAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<Enrollment?> GetByStudentAndClassAsync(int studentId, int classId)
        => await _context.Enrollments.FirstOrDefaultAsync(e => e.StudentID == studentId && e.ClassID == classId);

    public async Task UpdateAsync(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Enrollment enrollment)
    {
        _context.Enrollments.Remove(enrollment);
        await _context.SaveChangesAsync();
    }
}
