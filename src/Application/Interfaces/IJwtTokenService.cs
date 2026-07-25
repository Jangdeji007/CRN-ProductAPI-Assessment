using CRN.ProductAPI.Domain.Entities;

namespace CRN.ProductAPI.Application.Interfaces
{
    public interface IJwtTokenService
    {
        (string AccessToken, DateTime ExpiresAt) CreateAccessToken(User user, IReadOnlyList<string> roles);

        (string RefreshToken, DateTime ExpiresAt) CreateRefreshToken();

        string HashToken(string token);
    }
}
