using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class ClassSectionDAO
{
    private static ClassSectionDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public ClassSectionDAO(AppDbContext context) => _context = context;

    public static ClassSectionDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new ClassSectionDAO(context);
            return _instance;
        }
    }

    public IQueryable<ClassSection> GetAll()
        => _context.ClassSections.AsNoTracking();

    public async Task<ClassSection?> GetByIdAsync(int id)
        => await _context.ClassSections
            .Include(cs => cs.Course).Include(cs => cs.Lecturer)
            .FirstOrDefaultAsync(cs => cs.ClassID == id);

    public async Task<bool> HasEnrollmentsAsync(int classId)
        => await _context.Enrollments.AnyAsync(e => e.ClassID == classId);

    public async Task<int> GetEnrolledCountAsync(int classId)
        => await _context.Enrollments.CountAsync(e =>
            e.ClassID == classId && e.EnrollmentStatus == EnrollmentStatus.Approved);

    public async Task AddAsync(ClassSection classSection)
    {
        await _context.ClassSections.AddAsync(classSection);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ClassSection classSection)
    {
        _context.ClassSections.Update(classSection);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ClassSection classSection)
    {
        _context.ClassSections.Remove(classSection);
        await _context.SaveChangesAsync();
    }
}
