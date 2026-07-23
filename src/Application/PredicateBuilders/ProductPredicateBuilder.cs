using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Domain.Entities;
using LinqKit;
using System.Linq.Expressions;

namespace CRN.ProductAPI.Application.PredicateBuilders
{
    public static class ProductPredicateBuilder
    {
        public static Expression<Func<Product, bool>> Build(ProductFilterRequestModel request)
        {
            var predicate = PredicateBuilder.New<Product>(true);

            if (request.Id.HasValue)
                predicate = predicate.And(x => x.Id == request.Id.Value);

            if (!string.IsNullOrWhiteSpace(request.ProductName))
                predicate = predicate.And(x =>
                    x.ProductName.ToLower() == request.ProductName.Trim().ToLower());

            if (!string.IsNullOrWhiteSpace(request.CreatedBy))
                predicate = predicate.And(x =>
                    x.CreatedBy.ToLower() == request.CreatedBy.Trim().ToLower());

            if (request.FromCreatedOn.HasValue)
                predicate = predicate.And(x =>
                    x.CreatedOn >= request.FromCreatedOn.Value);

            if (request.ToCreatedOn.HasValue)
                predicate = predicate.And(x =>
                    x.CreatedOn <= request.ToCreatedOn.Value);

            return predicate;
        }
    }
}
