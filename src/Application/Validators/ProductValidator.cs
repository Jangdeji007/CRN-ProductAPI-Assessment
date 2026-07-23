using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;

namespace CRN.ProductAPI.Application.Validators
{
    internal class ProductValidator
    {
        public static Result<string>? ValidateAddProduct(AddProductRequestModel request)
        {
            if (request is null)
                return Result<string>.Failure("Request model cannot be null.", 400);

            if (string.IsNullOrWhiteSpace(request.ProductName))
                return Result<string>.Failure("Product name is required.", 400);

            if (string.IsNullOrWhiteSpace(request.CreatedBy))
                return Result<string>.Failure("CreatedBy is required.", 400);

            if (request.Item is null || !request.Item.Any())
                return Result<string>.Failure("At least one item is required.", 400);

            if (request.Item.Any(x => x.Quantity <= 0))
                return Result<string>.Failure("Item quantity must be greater than 0.", 400);

            return null;
        }

        public static Result<string>? ValidateGetAllProducts(ProductFilterRequestModel filter)
        {
            if (filter is null)
                return Result<string>.Failure("Request model cannot be null.", 400);

            if (filter.PageNumber < 1)
                return Result<string>.Failure("PageNumber must be greater than or equal to 1.", 400);

            if (filter.PageSize < 1 || filter.PageSize > 100)
                return Result<string>.Failure("PageSize must be between 1 and 100.", 400);

            if (filter.FromCreatedOn.HasValue
                && filter.ToCreatedOn.HasValue
                && filter.FromCreatedOn.Value > filter.ToCreatedOn.Value)
            {
                return Result<string>.Failure("FromCreatedOn must be less than or equal to ToCreatedOn.", 400);
            }

            return null;
        }
    }
}
