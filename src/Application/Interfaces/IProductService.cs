using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;

namespace CRN.ProductAPI.Application.Interfaces
{
    public interface IProductService
    {
       Task<Result<string>> AddProduct(AddProductRequestModel request, CancellationToken cancellationToken);
    }
}
