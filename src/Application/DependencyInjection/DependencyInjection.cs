using CRN.ProductAPI.Application.Interfaces;
using CRN.ProductAPI.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CRN.ProductAPI.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
