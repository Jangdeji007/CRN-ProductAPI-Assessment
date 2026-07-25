using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;

namespace CRN.ProductAPI.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseModel>> RegisterAsync(RegisterRequestModel request, CancellationToken cancellationToken = default);

        Task<Result<AuthResponseModel>> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken = default);

        Task<Result<AuthResponseModel>> RefreshAsync(RefreshTokenRequestModel request, CancellationToken cancellationToken = default);

        Task<Result<object?>> LogoutAsync(RefreshTokenRequestModel request, CancellationToken cancellationToken = default);
    }
}
