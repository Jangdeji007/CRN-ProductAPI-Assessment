using System.ComponentModel.DataAnnotations;

namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class AddProductRequestModel
    {
        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(200, ErrorMessage = "Product name must not exceed 200 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "CreatedBy is required.")]
        [MaxLength(100, ErrorMessage = "CreatedBy must not exceed 100 characters.")]
        public string CreatedBy { get; set; } = string.Empty;

        [Required(ErrorMessage = "At least one item is required.")]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public ICollection<AddItemRequestModel> Item { get; set; } = new List<AddItemRequestModel>();
    }
}
