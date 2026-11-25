using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken);

    Task<bool> AnyInCategoryAsync(int categoryId, CancellationToken cancellationToken);

    void Add(Product product);

    void Remove(Product product);
}
