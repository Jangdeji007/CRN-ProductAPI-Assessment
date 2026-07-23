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

            // Relies on SQL Server case-insensitive collation (default CI) so the unique index is usable.
            if (!string.IsNullOrWhiteSpace(request.ProductName))
            {
                var productName = request.ProductName.Trim();
                predicate = predicate.And(x => x.ProductName == productName);
            }

            if (!string.IsNullOrWhiteSpace(request.CreatedBy))
            {
                var createdBy = request.CreatedBy.Trim();
                predicate = predicate.And(x => x.CreatedBy == createdBy);
            }

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
