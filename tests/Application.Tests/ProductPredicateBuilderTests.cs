using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.PredicateBuilders;
using CRN.ProductAPI.Domain.Entities;

namespace CRN.ProductAPI.Application.Tests;

public class ProductPredicateBuilderTests
{
    [Fact]
    public void Build_MatchesProductNameUsingDirectEquality()
    {
        var predicate = ProductPredicateBuilder.Build(new ProductFilterRequestModel
        {
            ProductName = "  Widget  "
        }).Compile();

        var matching = new Product { ProductName = "Widget" };
        var other = new Product { ProductName = "Gadget" };

        Assert.True(predicate(matching));
        Assert.False(predicate(other));
    }
}
