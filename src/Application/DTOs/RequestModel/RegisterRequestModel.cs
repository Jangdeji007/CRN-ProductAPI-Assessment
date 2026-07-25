namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class RegisterRequestModel
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}
