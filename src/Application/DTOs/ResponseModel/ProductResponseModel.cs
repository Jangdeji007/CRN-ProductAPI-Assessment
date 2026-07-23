namespace CRN.ProductAPI.Application.DTOs.ResponseModel
{
    public class ProductResponseModel
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public ICollection<ItemResponseModel> Items { get; set; } = new List<ItemResponseModel>();
    }
}
