using System.Linq.Expressions;

namespace FUNewsManagement.DataAccess.Repositories;

public interface IGenericRepository<TEntity, TKey> where TEntity : class
{
    IQueryable<TEntity> Query();

    Task<TEntity?> GetByIdAsync(TKey id);

    Task<List<TEntity>> GetAllAsync();

    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}
