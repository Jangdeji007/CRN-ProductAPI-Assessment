using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;

namespace CRN.ProductAPI.Application.Interfaces
{
    public interface IProductService
    {
        Task<Result<AddProductResponseModel>> AddProduct(AddProductRequestModel request, CancellationToken cancellationToken);

        Task<Result<ProductResponseModel>> GetProductById(Guid id, CancellationToken cancellationToken);

        Task<Result<PagedResult<ProductResponseModel>>> GetAllProducts(ProductFilterRequestModel filter, CancellationToken cancellationToken);

        Task<Result<ProductResponseModel>> UpdateProduct(Guid id, UpdateProductRequestModel request, CancellationToken cancellationToken);

        Task<Result<object?>> DeleteProduct(Guid id, CancellationToken cancellationToken);
    }
}
