using Space.Domain.Entities;
using System.Linq.Expressions;

namespace Space.Infrastructure.Application;

public interface IBaseRepository<TEntity> where TEntity : class, IBaseEntity
{
    Task <IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default, bool asNoTracking = false, params Expression<Func<TEntity, object>>[] includes);
    Task BulkInsertOrUpdateAsync(IEnumerable<TEntity> entities, string? updateByProperty, CancellationToken cancellationToken = default);
}
