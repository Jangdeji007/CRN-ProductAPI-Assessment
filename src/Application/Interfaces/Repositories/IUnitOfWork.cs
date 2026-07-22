
namespace CRN.ProductAPI.Application.Interfaces.Repositories
{
        public interface IUnitOfWork : IAsyncDisposable
        {
            IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;

            Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        }
}
    