using System.Reflection;
using CRN.ProductAPI.API.Controllers;
using CRN.ProductAPI.Application.DTOs.RequestModel;
using CRN.ProductAPI.Application.DTOs.ResponseModel;
using CRN.ProductAPI.Application.Validators;
using CRN.ProductAPI.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Tests;

public class AuthEndpointTests
{
    [Fact]
    public void AuthController_UsesApiAuthRoute()
    {
        var route = typeof(AuthController).GetCustomAttribute<RouteAttribute>();

        Assert.NotNull(route);
        Assert.Equal("api/auth", route!.Template);
    }

    [Theory]
    [InlineData(nameof(ProductController.AddProductAsync))]
    [InlineData(nameof(ProductController.UpdateProductAsync))]
    [InlineData(nameof(ProductController.DeleteProductAsync))]
    public void ProductMutatingActions_RequireAdminRole(string methodName)
    {
        var method = typeof(ProductController).GetMethod(methodName);
        var authorize = method!.GetCustomAttribute<AuthorizeAttribute>();

        Assert.NotNull(authorize);
        Assert.Equal(RoleNames.Admin, authorize!.Roles);
    }

    [Theory]
    [InlineData(nameof(ProductController.GetAllProductsAsync))]
    [InlineData(nameof(ProductController.GetProductByIdAsync))]
    public void ProductReadActions_DoNotRequireAuthorize(string methodName)
    {
        var method = typeof(ProductController).GetMethod(methodName);
        var authorize = method!.GetCustomAttribute<AuthorizeAttribute>();

        Assert.Null(authorize);
    }

    [Fact]
    public void AuthResponseModel_ExposesRolesCollection()
    {
        var response = new AuthResponseModel
        {
            Roles = [RoleNames.Admin, RoleNames.Member]
        };

        Assert.Contains(RoleNames.Admin, response.Roles);
        Assert.Contains(RoleNames.Member, response.Roles);
        Assert.Equal(2, response.Roles.Count);
    }

    [Fact]
    public void RegisterRequestModel_IsInvalid_WhenRequiredFieldsMissing()
    {
        var model = new RegisterRequestModel();
        var validator = new RegisterRequestModelValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequestModel.Email));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequestModel.Password));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequestModel.FullName));
    }

    [Fact]
    public void LoginRequestModel_IsInvalid_WhenEmailAndPasswordMissing()
    {
        var model = new LoginRequestModel();
        var validator = new LoginRequestModelValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginRequestModel.Email));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(LoginRequestModel.Password));
    }

    [Fact]
    public void RefreshTokenRequestModel_IsInvalid_WhenTokenMissing()
    {
        var model = new RefreshTokenRequestModel();
        var validator = new RefreshTokenRequestModelValidator();

        var result = validator.Validate(model);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RefreshTokenRequestModel.RefreshToken));
    }
}
