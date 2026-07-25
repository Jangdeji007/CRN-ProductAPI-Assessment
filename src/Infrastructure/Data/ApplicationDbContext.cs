using CRN.ProductAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRN.ProductAPI.Infrastructure.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Product => Set<Product>();

    public DbSet<Item> Item => Set<Item>();

    public DbSet<User> User => Set<User>();

    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();

    public DbSet<Role> Role => Set<Role>();

    public DbSet<UserRole> UserRole => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
