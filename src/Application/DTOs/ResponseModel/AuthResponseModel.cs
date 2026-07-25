namespace CRN.ProductAPI.Application.DTOs.ResponseModel
{
    public class AuthResponseModel
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public string Email { get; set; } = string.Empty;

        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    }
}
