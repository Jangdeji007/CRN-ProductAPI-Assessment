
using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.Interfaces;
using CRN.ProductAPI.Application.Interfaces.Repositories;
using CRN.ProductAPI.Domain.Entities;

namespace CRN.ProductAPI.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork) : IProductService
    {
       private readonly IUnitOfWork _unitOfWork = unitOfWork;
       public async Task<Result<string>> AddProduct(AddProductRequestModel request, CancellationToken cancellationToken)
       {
            var existingProduct = await _unitOfWork
                                .GetRepository<Product>()
                                .FirstOrDefaultAsync(x => x.ProductName == request.ProductName, cancellationToken);

            if (existingProduct != null)
                return Result<string>.Failure("Product already exists.", 409);

            var productId = Guid.NewGuid();

            Product product = new()
            {
                Id = productId,
                ProductName = request.ProductName,
                CreatedBy = request.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                Item = request.Item.Select(item => new Item
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = item.Quantity
                }).ToList()
            };

            await _unitOfWork.GetRepository<Product>().AddAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<string>.Success("Product added successfully.");
       }

    }
}
