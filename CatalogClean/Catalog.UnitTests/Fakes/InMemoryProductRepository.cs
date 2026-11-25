using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;

namespace Catalog.UnitTests.Fakes;

internal sealed class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _products = [];

    public IReadOnlyList<Product> Items => _products;

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_products.FirstOrDefault(product => product.Id == id));
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return Task.FromResult(_products.Any(product => product.Code == code));
    }

    public Task<bool> AnyInCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_products.Any(product => product.CategoryId == categoryId));
    }

    public void Add(Product product)
    {
        GeneratedId.Assign(product, _products.Count + 1);
        _products.Add(product);
    }

    public void Remove(Product product) => _products.Remove(product);
}
