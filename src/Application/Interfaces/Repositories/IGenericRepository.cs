
using System.Linq.Expressions;

namespace CRN.ProductAPI.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        // Read
        IQueryable<T> GetAll();

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate,CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<T> Items, int TotalCount)> FindPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default,
            Expression<Func<T, object>>? orderByDescending = null);

        T? Find(params object[] keyValues);

        Task<T?> FindAsync(params object[] keyValues);

        // Create
        Task AddAsync(T entity,CancellationToken cancellationToken = default);

        Task AddRangeAsync(params T[] entities);

        // Update
        void Update(T entity);

        void UpdateRange(
            params T[] entities);

        // Delete
        void Delete(T entity);

        void DeleteRange(params T[] entities);

        // Raw SQL (Optional)
        IQueryable<T> FromSqlRaw(string sql, params object[] parameters);
    }
}
