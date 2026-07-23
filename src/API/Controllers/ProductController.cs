using CRN.ProductAPI.API.Extensions;
using CRN.ProductAPI.Application.DTOs.RequestModel;
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
            var result = await productService.AddProduct(requestModel, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] ProductFilterRequestModel filter, CancellationToken cancellationToken)
        {
            var result = await productService.GetAllProducts(filter, cancellationToken);

            return result.ToActionResult();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await productService.GetProductById(id, cancellationToken);

            return result.ToActionResult();
        }
    }
}
