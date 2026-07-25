using CRN.ProductAPI.Application.Interfaces;
using CRN.ProductAPI.Domain.Constants;
using CRN.ProductAPI.Domain.Entities;
using CRN.ProductAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CRN.ProductAPI.Infrastructure.Identity
{
    public static class DbSeeder
    {
        public const string AdminEmail = "admin@crn.local";
        public const string AdminPassword = "Admin@123";

        public static readonly Guid AdminRoleId = new("11111111-1111-1111-1111-111111111111");
        public static readonly Guid MemberRoleId = new("22222222-2222-2222-2222-222222222222");

        public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            await context.Database.MigrateAsync(cancellationToken);

            await EnsureRolesAsync(context, cancellationToken);

            if (await context.User.AnyAsync(cancellationToken))
                return;

            var adminRole = await context.Role.FirstAsync(r => r.Name == RoleNames.Admin, cancellationToken);

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = AdminEmail,
                FullName = "System Admin",
                CreatedOn = DateTime.UtcNow
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, AdminPassword);

            context.User.Add(admin);
            context.UserRole.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            });

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Seeded admin user {Email} with role {Role}", AdminEmail, RoleNames.Admin);
        }

        private static async Task EnsureRolesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            if (!await context.Role.AnyAsync(r => r.Name == RoleNames.Admin, cancellationToken))
            {
                context.Role.Add(new Role
                {
                    Id = AdminRoleId,
                    Name = RoleNames.Admin,
                    Description = "Full access to mutating product operations"
                });
            }

            if (!await context.Role.AnyAsync(r => r.Name == RoleNames.Member, cancellationToken))
            {
                context.Role.Add(new Role
                {
                    Id = MemberRoleId,
                    Name = RoleNames.Member,
                    Description = "Standard registered user"
                });
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
