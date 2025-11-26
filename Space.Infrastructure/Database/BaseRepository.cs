using Microsoft.EntityFrameworkCore;
using Space.Infrastructure.Application;
using System.Linq.Expressions;

namespace Space.Infrastructure.Database
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly SpaceDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(SpaceDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAll(
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            if (asNoTracking) query = query.AsNoTracking();
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            return query;
        }

        public virtual IQueryable<TEntity> Find(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = false,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet.Where(predicate);
            if (asNoTracking) query = query.AsNoTracking();
            if (includes != null)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }
            return query;
        }

        public virtual void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public virtual async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
