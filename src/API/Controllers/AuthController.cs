using CRN.ProductAPI.API.Extensions;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            [FromBody] RegisterRequestModel request,
            CancellationToken cancellationToken)
        {
            var result = await authService.RegisterAsync(request, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginRequestModel request,
            CancellationToken cancellationToken)
        {
            var result = await authService.LoginAsync(request, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync(
            [FromBody] RefreshTokenRequestModel request,
            CancellationToken cancellationToken)
        {
            var result = await authService.RefreshAsync(request, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync(
            [FromBody] RefreshTokenRequestModel request,
            CancellationToken cancellationToken)
        {
            var result = await authService.LogoutAsync(request, cancellationToken);
            return result.ToActionResult();
        }
    }
}
