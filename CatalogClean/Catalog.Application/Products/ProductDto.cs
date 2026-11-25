namespace Catalog.Application.Products;

public sealed record ProductDto(
    int Id,
    string Code,
    string Name,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTime CreatedAt,
    int CategoryId,
    string CategoryName);
