using CRN.ProductAPI.Application.Comman;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            var response = new
            {
                result.IsSuccess,
                result.StatusCode,
                result.Message,
                Data = result.IsSuccess ? result.Data : default // Use default(T) instead of null
            };

            return new ObjectResult(response) { StatusCode = result.StatusCode };
        }
    }
}
