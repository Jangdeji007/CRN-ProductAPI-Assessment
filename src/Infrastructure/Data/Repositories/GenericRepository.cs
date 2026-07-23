using CRN.ProductAPI.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRN.ProductAPI.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _entity;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _entity = _context.Set<T>();
        }

        #region Read

        public IQueryable<T> GetAll()
        {
            return _entity.AsNoTracking();
        }

        public async Task<T?> FirstOrDefaultAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _entity
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _entity
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<T> Items, int TotalCount)> FindPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default,
            Expression<Func<T, object>>? orderByDescending = null)
        {
            var query = _entity.AsNoTracking().Where(predicate);

            var totalCount = await query.CountAsync(cancellationToken);

            if (orderByDescending is not null)
                query = query.OrderByDescending(orderByDescending);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public T? Find(params object[] keyValues)
        {
            return _entity.Find(keyValues);
        }

        public async Task<T?> FindAsync(params object[] keyValues)
        {
            return await _entity.FindAsync(keyValues);
        }

        #endregion

        #region Create

        public async Task AddAsync(
            T entity,
            CancellationToken cancellationToken = default)
        {
            await _entity.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(params T[] entities)
        {
            await _entity.AddRangeAsync(entities);
        }

        #endregion

        #region Update

        public void Update(T entity)
        {
            _entity.Update(entity);
        }

        public void UpdateRange(params T[] entities)
        {
            _entity.UpdateRange(entities);
        }

        #endregion

        #region Delete

        public void Delete(T entity)
        {
            _entity.Remove(entity);
        }

        public void DeleteRange(params T[] entities)
        {
            _entity.RemoveRange(entities);
        }

        #endregion

        #region Raw SQL

        public IQueryable<T> FromSqlRaw(
            string sql,
            params object[] parameters)
        {
            return _entity.FromSqlRaw(sql, parameters);
        }

        #endregion
    }
}
