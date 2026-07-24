namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class AddProductRequestModel
    {
        public string ProductName { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public ICollection<AddItemRequestModel> Item { get; set; } = new List<AddItemRequestModel>();
    }
}
