using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Space.Domain.Entities;
using Space.Infrastructure.Application;
using System.Linq.Expressions;

namespace Space.Infrastructure.Database;

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IBaseEntity
{
    protected readonly SpaceDbContext _dbContext;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(SpaceDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken,
        bool asNoTracking = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        if (asNoTracking)
            query = query.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task BulkInsertOrUpdateAsync(
        IEnumerable<TEntity> entities,
        string? updateByProperty = null,
        CancellationToken cancellationToken = default)
    {
        var bulkConfig = new BulkConfig
        {
            SetOutputIdentity = true,
            IncludeGraph = false
        };

        if (!string.IsNullOrEmpty(updateByProperty))
            bulkConfig.UpdateByProperties = new List<string> { updateByProperty };

        await _dbContext.BulkInsertOrUpdateAsync(
            entities.ToList(),
            bulkConfig,
            cancellationToken: cancellationToken
        );
    }
}
