using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;
using CRN.ProductAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductAsync([FromBody] AddProductRequestModel requestModel, CancellationToken cancellationToken)
        {
            Result<AddProductResponseModel> result = await productService.AddProduct(requestModel, cancellationToken);

            return StatusCode(result.StatusCode, result);
        }
    }
}
