using FUCourseManagementSystem.BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class LecturerDAO
{
    private static LecturerDAO? _instance;
    private static readonly object _lock = new();
    private readonly AppDbContext _context;

    public LecturerDAO(AppDbContext context) => _context = context;

    public static LecturerDAO Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new LecturerDAO(context);
            return _instance;
        }
    }

    public IQueryable<Lecturer> GetAll()
        => _context.Lecturers.AsNoTracking();

    public async Task<Lecturer?> GetByIdAsync(int id)
        => await _context.Lecturers.Include(l => l.Department).Include(l => l.Account)
            .FirstOrDefaultAsync(l => l.LecturerID == id);

    public async Task<Lecturer?> GetByAccountIdAsync(int accountId)
        => await _context.Lecturers.Include(l => l.Department)
            .FirstOrDefaultAsync(l => l.AccountID == accountId);

    public async Task AddAsync(Lecturer lecturer)
    {
        await _context.Lecturers.AddAsync(lecturer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Lecturer lecturer)
    {
        _context.Lecturers.Update(lecturer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Lecturer lecturer)
    {
        _context.Lecturers.Remove(lecturer);
        await _context.SaveChangesAsync();
    }
}
