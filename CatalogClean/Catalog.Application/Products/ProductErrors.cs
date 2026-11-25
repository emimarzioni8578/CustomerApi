using Catalog.Application.Results;

namespace Catalog.Application.Products;

public static class ProductErrors
{
    public static Error NotFound(int id) => Error.NotFound($"Producto con id '{id}' no existe.");

    public static Error CodeAlreadyExists(string code) => Error.Conflict($"Ya existe un producto con el código '{code}'.");

    public static Error InsufficientStock(int stock, int quantity) =>
        Error.Conflict($"Stock insuficiente: hay {stock} unidades y se intentó descontar {-quantity}.");
}
