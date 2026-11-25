using Catalog.Api.Contracts;
using Catalog.Api.Mappers;
using Catalog.Application.Products;
using Catalog.Application.Products.Commands;
using Catalog.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts(
        [FromQuery] int? categoryId,
        [FromQuery] bool onlyActive,
        CancellationToken cancellationToken)
    {
        var products = await sender.Send(new GetProductsQuery(categoryId, onlyActive), cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return CreatedAtAction(nameof(GetProduct), new { id = result.Value }, result.Value);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProductCommand(id, request.CategoryId, request.Name, request.Price),
            cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return NoContent();
    }

    [HttpPatch("{id:int}/stock")]
    public async Task<IActionResult> AdjustStock(int id, AdjustProductStockRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AdjustProductStockCommand(id, request.Quantity), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteProductCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return NoContent();
    }
}
