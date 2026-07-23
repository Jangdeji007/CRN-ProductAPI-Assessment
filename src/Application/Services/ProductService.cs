using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;
using CRN.ProductAPI.Application.Interfaces;
using CRN.ProductAPI.Application.Interfaces.Repositories;
using CRN.ProductAPI.Application.PredicateBuilders;
using CRN.ProductAPI.Application.Validators;
using CRN.ProductAPI.Domain.Entities;

namespace CRN.ProductAPI.Application.Services
{
    public class ProductService(IUnitOfWork unitOfWork) : IProductService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result<AddProductResponseModel>> AddProduct(AddProductRequestModel request, CancellationToken cancellationToken)
        {
            var validationError = ProductValidator.ValidateAddProduct(request);
            if (validationError is not null)
                return Result<AddProductResponseModel>.Failure(validationError.Message!, validationError.StatusCode);

            var productRepository = _unitOfWork.GetRepository<Product>();

            var filter = new ProductFilterRequestModel
            {
                ProductName = request.ProductName
            };

            var predicate = ProductPredicateBuilder.Build(filter);

            var existingProduct = await productRepository.FirstOrDefaultAsync(predicate, cancellationToken);

            if (existingProduct is not null)
                return Result<AddProductResponseModel>.Failure("Product already exists.", 409);

            var product = MapToProduct(request);

            await productRepository.AddAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<AddProductResponseModel>.Success(
                new AddProductResponseModel
                {
                    ProductId = product.Id,
                    Message = "Product added successfully."
                },
                statusCode: 201);
        }

        private static Product MapToProduct(AddProductRequestModel request)
        {
            var productId = Guid.NewGuid();

            return new Product
            {
                Id = productId,
                ProductName = request.ProductName.Trim(),
                CreatedBy = request.CreatedBy.Trim(),
                CreatedOn = DateTime.UtcNow,
                Item = request.Item.Select(x => new Item
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = x.Quantity
                }).ToList()
            };
        }
    }
}
