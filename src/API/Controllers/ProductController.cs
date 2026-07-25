using CRN.ProductAPI.API.Extensions;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProductAsync(Guid id, [FromBody] UpdateProductRequestModel requestModel, CancellationToken cancellationToken)
        {
            var result = await productService.UpdateProduct(id, requestModel, cancellationToken);

            return result.ToActionResult();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProductAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await productService.DeleteProduct(id, cancellationToken);

            return result.ToActionResult();
        }
    }
}
