using Microsoft.AspNetCore.Mvc;
using ProductCatalogApi.Business.Results;

namespace ProductCatalogApi.Mappers
{
    public static class ResultMapper
    {
        public static ObjectResult ToProblem(this Error error) 
        {
            var (status, title) = error.Type switch
            {
                ErrorType.Validation => (StatusCodes.Status400BadRequest, "Validation Error"),
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
                ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = error.Message
            };
            return new ObjectResult(problem)
            {
                StatusCode = status
            };
        }
    }

}
