namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class UpdateProductRequestModel
    {
        public string ProductName { get; set; } = string.Empty;

        public string ModifiedBy { get; set; } = string.Empty;

        public ICollection<AddItemRequestModel> Item { get; set; } = new List<AddItemRequestModel>();
    }
}
