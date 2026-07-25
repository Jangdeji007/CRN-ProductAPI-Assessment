using CRN.ProductAPI.Domain.Entities;

namespace CRN.ProductAPI.Application.Interfaces
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);

        bool VerifyPassword(User user, string hashedPassword, string providedPassword);
    }
}
