using ProductCatalogApi.Entities;
using ProductCatalogApi.Models;
namespace ProductCatalogApi.Mappers
{
    public static class ProductMapper
    {
        public static ProductResponse ToProductResponse(this Producto product)
        {
            return new ProductResponse
            {
                ProductoId = product.ProductoId,
                CategoriaId = product.CategoriaId,
                Codigo = product.Codigo,
                Nombre = product.Nombre,
                Precio = product.Precio,
                Stock = product.Stock,
                Activo = product.Activo,
                FechaAlta = product.FechaAlta
            };
        }

        public static Producto ToProducto(this ProductRequest productRequest)
        {
            return new Producto
            {
                Codigo = productRequest.Codigo,
                Nombre = productRequest.Nombre,
                Precio = productRequest.Precio,
                Stock = productRequest.Stock,
                CategoriaId = productRequest.CategoriaId,
                Activo = true, // Set default value for Activo
                FechaAlta = DateTime.UtcNow // Set default value for FechaAlta
            };
        }

        public static IEnumerable<ProductResponse> ToDtoList(this IEnumerable<Producto> productos)
        {
            return productos.Select(producto => producto.ToProductResponse());
        }
    }
}
