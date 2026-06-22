using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class StudentDAO
{
    private static StudentDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public StudentDAO(AppDbContext context) => _context = context;

    public static StudentDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new StudentDAO(context);
            return _instance;
        }
    }

    public IQueryable<Student> GetAll()
        => _context.Students.Include(s => s.Account).AsNoTracking();

    public async Task<Student?> GetByIdAsync(int id)
        => await _context.Students.Include(s => s.Account).FirstOrDefaultAsync(s => s.StudentID == id);

    public async Task<Student?> GetByAccountIdAsync(int accountId)
        => await _context.Students.FirstOrDefaultAsync(s => s.AccountID == accountId);

    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Student student)
    {
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
    }
}
