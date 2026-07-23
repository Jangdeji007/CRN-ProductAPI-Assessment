namespace CRN.ProductAPI.Application.DTOs.ResponseModel
{
    public class AddProductResponseModel
    {
        public Guid ProductId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
