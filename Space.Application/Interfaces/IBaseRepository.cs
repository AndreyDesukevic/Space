using System.Linq.Expressions;

namespace Space.Infrastructure.Application;

public interface IBaseRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll(bool asNoTracking = false, params Expression<Func<TEntity, object>>[] includes);
    IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false, params Expression<Func<TEntity, object>>[] includes);
    void Add(TEntity entity);
    void AddRange(IEnumerable<TEntity> entities);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    Task SaveChangesAsync();
}
