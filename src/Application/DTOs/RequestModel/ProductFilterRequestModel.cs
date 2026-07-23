namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class ProductFilterRequestModel
    {
        public Guid? Id { get; set; }

        public string? ProductName { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? FromCreatedOn { get; set; }

        public DateTime? ToCreatedOn { get; set; }
    }
}
