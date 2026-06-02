using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FUCourseManagementSystem.DataAccess.DAOs;

public class GenericDAO<T> where T : class
{
    private static GenericDAO<T>? _instance;
    private static readonly object _lock = new();

    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericDAO(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public static GenericDAO<T> Instance(AppDbContext context)
    {
        lock (_lock)
        {
            _instance ??= new GenericDAO<T>(context);
            return _instance;
        }
    }

    public IQueryable<T> GetAll() => _dbSet.AsNoTracking();

    public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        => _dbSet.AsNoTracking().Where(predicate);

    public async Task<T?> GetByIdAsync(object id)
        => await _dbSet.FindAsync(id);

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
