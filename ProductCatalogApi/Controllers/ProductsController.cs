using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Business;
using ProductCatalogApi.DataAccess;
using ProductCatalogApi.DataAccess.Repositories;
using ProductCatalogApi.Entities;
using ProductCatalogApi.Mappers;
using ProductCatalogApi.Models;

namespace ProductCatalogApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products.ToDtoList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        return Ok(result.Value!.ToProductResponse());
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutProduct(int id, Producto producto)
    {
        if (id != producto.ProductoId)
        {
            return BadRequest();
        }

        await _productService.UpdateProductAsync(id, producto);

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> PostProduct(ProductRequest productRequest)
    {
        var producto = productRequest.ToProducto();
        var result = await _productService.AddProductAsync(producto);

        if (result.IsFailure)
        {
            return result.Error!.ToProblem();
        }

        var product = result.Value!;

        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductoId }, product.ToProductResponse());
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {   
        await _productService.DeleteProductAsync(id);

        return NoContent();
    }
}