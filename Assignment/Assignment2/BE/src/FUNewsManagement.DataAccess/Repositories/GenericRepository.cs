using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagement.DataAccess.Repositories;

public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : class
{
    protected readonly FUNewsManagementContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public GenericRepository(FUNewsManagementContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query() => DbSet.AsQueryable();

    public async Task<TEntity?> GetByIdAsync(TKey id) => await DbSet.FindAsync(id);

    public async Task<List<TEntity>> GetAllAsync() => await DbSet.ToListAsync();

    public async Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate) =>
        await DbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(TEntity entity) => await DbSet.AddAsync(entity);

    public void Update(TEntity entity) => DbSet.Update(entity);

    public void Remove(TEntity entity) => DbSet.Remove(entity);
}
