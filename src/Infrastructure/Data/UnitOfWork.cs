using CRN.ProductAPI.Application.Exceptions;
using CRN.ProductAPI.Application.Interfaces.Repositories;
using CRN.ProductAPI.Infrastructure.Data.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CRN.ProductAPI.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private const int SqlServerUniqueIndexViolation = 2601;
    private const int SqlServerUniqueConstraintViolation = 2627;

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
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            throw new DuplicateResourceException(
                "A resource with the same unique key already exists.",
                ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception)
    {
        if (exception.InnerException is not SqlException sqlException)
            return false;

        return sqlException.Number is SqlServerUniqueIndexViolation
            or SqlServerUniqueConstraintViolation;
    }
}
