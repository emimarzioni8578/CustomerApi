namespace Catalog.Api.Contracts;

public sealed record UpdateProductRequest(int CategoryId, string Name, decimal Price);

public sealed record AdjustProductStockRequest(int Quantity);
