using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CRN.ProductAPI.API.Controllers;
using CRN.ProductAPI.API.Extensions;
using CRN.ProductAPI.Application.Comman;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Tests;

public class AddProductEndpointTests
{
    [Fact]
    public void ProductController_UsesPluralProductsRoute()
    {
        var route = typeof(ProductController).GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(route);
        Assert.Equal("api/products", route!.Template);
    }

    [Fact]
    public void AddProductAsync_IsHttpPostWithoutVerbInTemplate()
    {
        var method = typeof(ProductController).GetMethod(nameof(ProductController.AddProductAsync));
        var httpPost = method!.GetCustomAttribute<HttpPostAttribute>();

        Assert.NotNull(httpPost);
        Assert.True(string.IsNullOrEmpty(httpPost!.Template));
    }

    [Fact]
    public void AddProductRequestModel_IsInvalid_WhenRequiredFieldsMissing()
    {
        var model = new AddProductRequestModel();
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddProductRequestModel.ProductName)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddProductRequestModel.CreatedBy)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddProductRequestModel.Item)));
    }

    [Fact]
    public void AddItemRequestModel_IsInvalid_WhenQuantityIsZero()
    {
        var model = new AddItemRequestModel { Quantity = 0 };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(AddItemRequestModel.Quantity)));
    }

    [Fact]
    public void ToActionResult_UsesHttpStatusAndOmitsStatusCodeFromBody()
    {
        var result = Result<AddProductResponseModel>.Success(
            new AddProductResponseModel { ProductId = Guid.NewGuid(), Message = "Product added successfully." },
            statusCode: 201);

        var actionResult = result.ToActionResult();
        var objectResult = Assert.IsType<ObjectResult>(actionResult);

        Assert.Equal(201, objectResult.StatusCode);

        var json = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        Assert.DoesNotContain("StatusCode", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("isSuccess", json, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ProductId", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToActionResult_Failure_ReturnsConflictWithoutData()
    {
        var result = Result<AddProductResponseModel>.Failure("Product already exists.", 409);

        var actionResult = result.ToActionResult();
        var objectResult = Assert.IsType<ObjectResult>(actionResult);

        Assert.Equal(409, objectResult.StatusCode);

        var json = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
        Assert.Contains("Product already exists.", json);
        Assert.Contains("\"Data\":null", json, StringComparison.OrdinalIgnoreCase);
    }
}
