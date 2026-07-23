using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;
using CRN.ProductAPI.Application.Exceptions;
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

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DuplicateResourceException)
            {
                return Result<AddProductResponseModel>.Failure("Product already exists.", 409);
            }

            var response = new AddProductResponseModel { ProductId = product.Id, Message = "Product added successfully." };

            return Result<AddProductResponseModel>.Success(response, statusCode: 201);
        }

        public async Task<Result<ProductResponseModel>> GetProductById(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                return Result<ProductResponseModel>.Failure("Product id is required.", 400);

            var productRepository = _unitOfWork.GetRepository<Product>();

            var filter = new ProductFilterRequestModel { Id = id };
            var predicate = ProductPredicateBuilder.Build(filter);

            var product = await productRepository.FirstOrDefaultAsync(predicate, cancellationToken);

            if (product is null)
                return Result<ProductResponseModel>.Failure("Product not found.", 404);

            var itemRepository = _unitOfWork.GetRepository<Item>();
            var items = await itemRepository.FindAllAsync(i => i.ProductId == id, cancellationToken);

            var response = MapToProductResponse(product, items);

            return Result<ProductResponseModel>.Success(response);
        }

        public async Task<Result<PagedResult<ProductResponseModel>>> GetAllProducts(ProductFilterRequestModel filter, CancellationToken cancellationToken)
        {
            var validationError = ProductValidator.ValidateGetAllProducts(filter);

            if (validationError is not null)
                return Result<PagedResult<ProductResponseModel>>.Failure(validationError.Message!, validationError.StatusCode);

            var productRepository = _unitOfWork.GetRepository<Product>();
            var predicate = ProductPredicateBuilder.Build(filter);

            var (products, totalCount) = await productRepository.FindPagedAsync(predicate, filter.PageNumber, filter.PageSize, cancellationToken, p => p.CreatedOn);

            if (products.Count == 0)
            {
                return Result<PagedResult<ProductResponseModel>>.Success(new PagedResult<ProductResponseModel>
                {
                    Items = Array.Empty<ProductResponseModel>(),
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalCount = totalCount
                });
            }

            var productIds = products.Select(p => p.Id).ToList();
            var itemRepository = _unitOfWork.GetRepository<Item>();
            var items = await itemRepository.FindAllAsync(i => productIds.Contains(i.ProductId), cancellationToken);
            var itemsByProductId = items.GroupBy(i => i.ProductId).ToDictionary(g => g.Key, g => (IReadOnlyList<Item>)g.ToList());

            var responseItems = products.Select(product =>
            {
                itemsByProductId.TryGetValue(product.Id, out var productItems);
                return MapToProductResponse(product, productItems ?? Array.Empty<Item>());
            }).ToList();

            return Result<PagedResult<ProductResponseModel>>.Success(new PagedResult<ProductResponseModel>
            {
                Items = responseItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            });
        }

        private static ProductResponseModel MapToProductResponse(Product product, IReadOnlyList<Item> items)
        {
            return new ProductResponseModel
            {
                Id = product.Id,
                ProductName = product.ProductName,
                CreatedBy = product.CreatedBy,
                CreatedOn = product.CreatedOn,
                ModifiedBy = product.ModifiedBy,
                ModifiedOn = product.ModifiedOn,
                Items = items.Select(i => new ItemResponseModel
                {
                    Id = i.Id,
                    Quantity = i.Quantity
                }).ToList()
            };
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
