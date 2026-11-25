using Catalog.Application.Results;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Mappers;

public static class ResultMapper
{
    public static ObjectResult ToProblem(this Error error)
    {
        var (status, title) = error.Type switch
        {
            ErrorType.Validation => (StatusCodes.Status400BadRequest, "La solicitud no es válida."),
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Recurso no encontrado."),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflicto con el estado actual."),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor.")
        };

        ProblemDetails problem = error.Fields is null
            ? new ProblemDetails()
            : new ValidationProblemDetails(error.Fields);

        problem.Status = status;
        problem.Title = title;
        problem.Detail = error.Message;

        return new ObjectResult(problem)
        {
            StatusCode = status
        };
    }
}
