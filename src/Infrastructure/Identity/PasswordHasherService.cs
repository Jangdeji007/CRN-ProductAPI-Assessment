using CRN.ProductAPI.Application.Interfaces;
using CRN.ProductAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CRN.ProductAPI.Infrastructure.Identity
{
    public class PasswordHasherService : IPasswordHasherService
    {
        private readonly PasswordHasher<User> _passwordHasher = new();

        public string HashPassword(User user, string password)
            => _passwordHasher.HashPassword(user, password);

        public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
            return result is PasswordVerificationResult.Success
                or PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
