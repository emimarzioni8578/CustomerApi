using Catalog.Api.Contracts;
using Catalog.Api.Mappers;
using Catalog.Application.Categories;
using Catalog.Application.Categories.Commands;
using Catalog.Application.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await sender.Send(new GetCategoriesQuery(), cancellationToken);

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return CreatedAtAction(nameof(GetCategory), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateCategoryCommand(id, request.Name, request.Description), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return NoContent();
    }
}
