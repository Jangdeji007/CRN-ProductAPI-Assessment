using CRN.ProductAPI.Application.Interfaces.Repositories;
using CRN.ProductAPI.Infrastructure.Data.Repositories;
using System.Collections;

namespace CRN.ProductAPI.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = [];

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
    {
        Type type = typeof(TEntity);

        if (!_repositories.TryGetValue(type, out object? repository))
        {
            repository = new GenericRepository<TEntity>(_context);
            _repositories[type] = repository;
        }

        return (IGenericRepository<TEntity>)repository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
