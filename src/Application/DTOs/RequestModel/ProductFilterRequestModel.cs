namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class ProductFilterRequestModel
    {
        public Guid? Id { get; set; }

        public string? ProductName { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? FromCreatedOn { get; set; }

        public DateTime? ToCreatedOn { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
