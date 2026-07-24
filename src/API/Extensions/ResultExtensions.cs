using CRN.ProductAPI.Application.Comman;
using Microsoft.AspNetCore.Mvc;

namespace CRN.ProductAPI.API.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess && result.StatusCode == StatusCodes.Status204NoContent)
                return new NoContentResult();

            var response = new
            {
                result.IsSuccess,
                result.Message,
                Data = result.IsSuccess ? result.Data : default
            };

            return new ObjectResult(response) { StatusCode = result.StatusCode };
        }
    }
}
