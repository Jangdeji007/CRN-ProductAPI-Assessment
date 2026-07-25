using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;
using CRN.ProductAPI.Application.Exceptions;
using CRN.ProductAPI.Application.Interfaces;
using CRN.ProductAPI.Application.Interfaces.Repositories;
using CRN.ProductAPI.Domain.Constants;
using CRN.ProductAPI.Domain.Entities;
using FluentValidation;

namespace CRN.ProductAPI.Application.Services
{
    public class AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService,
        IPasswordHasherService passwordHasher,
        IValidator<RegisterRequestModel> registerValidator,
        IValidator<LoginRequestModel> loginValidator,
        IValidator<RefreshTokenRequestModel> refreshValidator) : IAuthService
    {
        public async Task<Result<AuthResponseModel>> RegisterAsync(RegisterRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = await registerValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return Result<AuthResponseModel>.Failure(validation.Errors[0].ErrorMessage, 400);

            var userRepository = unitOfWork.GetRepository<User>();
            var roleRepository = unitOfWork.GetRepository<Role>();
            var userRoleRepository = unitOfWork.GetRepository<UserRole>();

            var email = request.Email.Trim().ToLowerInvariant();

            var existing = await userRepository.FirstOrDefaultAsync(
                u => u.Email == email,
                cancellationToken);

            if (existing is not null)
                return Result<AuthResponseModel>.Failure("Email is already registered.", 409);

            var memberRole = await roleRepository.FirstOrDefaultAsync(
                r => r.Name == RoleNames.Member,
                cancellationToken);

            if (memberRole is null)
                return Result<AuthResponseModel>.Failure("Default Member role is not configured.", 500);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                FullName = request.FullName.Trim(),
                CreatedOn = DateTime.UtcNow
            };

            user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

            await userRepository.AddAsync(user, cancellationToken);
            await userRoleRepository.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = memberRole.Id
            }, cancellationToken);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DuplicateResourceException)
            {
                return Result<AuthResponseModel>.Failure("Email is already registered.", 409);
            }

            return await IssueTokensAsync(user, cancellationToken, statusCode: 201);
        }

        public async Task<Result<AuthResponseModel>> LoginAsync(LoginRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = await loginValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return Result<AuthResponseModel>.Failure(validation.Errors[0].ErrorMessage, 400);

            var email = request.Email.Trim().ToLowerInvariant();
            var userRepository = unitOfWork.GetRepository<User>();

            var user = await userRepository.FirstOrDefaultAsync(
                u => u.Email == email,
                cancellationToken);

            if (user is null || !passwordHasher.VerifyPassword(user, user.PasswordHash, request.Password))
                return Result<AuthResponseModel>.Failure("Invalid email or password.", 401);

            return await IssueTokensAsync(user, cancellationToken);
        }

        public async Task<Result<AuthResponseModel>> RefreshAsync(RefreshTokenRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = await refreshValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return Result<AuthResponseModel>.Failure(validation.Errors[0].ErrorMessage, 400);

            var tokenHash = jwtTokenService.HashToken(request.RefreshToken);
            var refreshRepository = unitOfWork.GetRepository<RefreshToken>();
            var userRepository = unitOfWork.GetRepository<User>();

            var storedToken = await refreshRepository.FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash,
                cancellationToken);

            if (storedToken is null)
                return Result<AuthResponseModel>.Failure("Invalid refresh token.", 401);

            if (storedToken.IsRevoked)
            {
                await RevokeAllUserTokensAsync(storedToken.UserId, cancellationToken);
                return Result<AuthResponseModel>.Failure("Refresh token reuse detected. All sessions revoked.", 401);
            }

            if (storedToken.IsExpired)
                return Result<AuthResponseModel>.Failure("Refresh token has expired.", 401);

            var user = await userRepository.FindAsync(storedToken.UserId);
            if (user is null)
                return Result<AuthResponseModel>.Failure("Invalid refresh token.", 401);

            var (newRefreshToken, refreshExpiresAt) = jwtTokenService.CreateRefreshToken();
            var newRefreshHash = jwtTokenService.HashToken(newRefreshToken);

            storedToken.RevokedOn = DateTime.UtcNow;
            storedToken.ReplacedByTokenHash = newRefreshHash;
            refreshRepository.Update(storedToken);

            var replacement = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = newRefreshHash,
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = refreshExpiresAt
            };

            await refreshRepository.AddAsync(replacement, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var roles = await GetUserRoleNamesAsync(user.Id, cancellationToken);
            var (accessToken, accessExpiresAt) = jwtTokenService.CreateAccessToken(user, roles);

            return Result<AuthResponseModel>.Success(new AuthResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = accessExpiresAt,
                Email = user.Email,
                Roles = roles
            });
        }

        public async Task<Result<object?>> LogoutAsync(RefreshTokenRequestModel request, CancellationToken cancellationToken = default)
        {
            var validation = await refreshValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return Result<object?>.Failure(validation.Errors[0].ErrorMessage, 400);

            var tokenHash = jwtTokenService.HashToken(request.RefreshToken);
            var refreshRepository = unitOfWork.GetRepository<RefreshToken>();

            var storedToken = await refreshRepository.FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash,
                cancellationToken);

            if (storedToken is null || storedToken.IsRevoked)
                return Result<object?>.Success(null, "Logged out.", 204);

            storedToken.RevokedOn = DateTime.UtcNow;
            refreshRepository.Update(storedToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<object?>.Success(null, "Logged out.", 204);
        }

        private async Task<Result<AuthResponseModel>> IssueTokensAsync(User user, CancellationToken cancellationToken,
            int statusCode = 200)
        {
            var roles = await GetUserRoleNamesAsync(user.Id, cancellationToken);
            var (accessToken, accessExpiresAt) = jwtTokenService.CreateAccessToken(user, roles);
            var (refreshToken, refreshExpiresAt) = jwtTokenService.CreateRefreshToken();

            var refreshEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = jwtTokenService.HashToken(refreshToken),
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = refreshExpiresAt
            };

            var refreshRepository = unitOfWork.GetRepository<RefreshToken>();
            await refreshRepository.AddAsync(refreshEntity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AuthResponseModel>.Success(new AuthResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = accessExpiresAt,
                Email = user.Email,
                Roles = roles
            }, statusCode: statusCode);
        }

        private async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(Guid userId,CancellationToken cancellationToken)
        {
            var userRoles = await unitOfWork.GetRepository<UserRole>()
                .FindAllAsync(ur => ur.UserId == userId, cancellationToken);

            if (userRoles.Count == 0)
                return Array.Empty<string>();

            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            var roles = await unitOfWork.GetRepository<Role>()
                .FindAllAsync(r => roleIds.Contains(r.Id), cancellationToken);

            return roles
                .Select(r => r.Name)
                .OrderBy(n => n)
                .ToList();
        }

        private async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken)
        {
            var refreshRepository = unitOfWork.GetRepository<RefreshToken>();
            var tokens = await refreshRepository.FindAllAsync(
                t => t.UserId == userId && t.RevokedOn == null,
                cancellationToken);

            if (tokens.Count == 0)
                return;

            var now = DateTime.UtcNow;
            foreach (var token in tokens)
                token.RevokedOn = now;

            refreshRepository.UpdateRange(tokens.ToArray());
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
