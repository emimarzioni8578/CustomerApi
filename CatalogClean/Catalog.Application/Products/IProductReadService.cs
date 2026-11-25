namespace Catalog.Application.Products;

public interface IProductReadService
{
    Task<IReadOnlyList<ProductDto>> ListAsync(int? categoryId, bool onlyActive, CancellationToken cancellationToken);

    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
