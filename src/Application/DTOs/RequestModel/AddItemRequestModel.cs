using System.ComponentModel.DataAnnotations;

namespace CRN.ProductAPI.Application.DTOs.RequestModel
{
    public class AddItemRequestModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Item quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}
