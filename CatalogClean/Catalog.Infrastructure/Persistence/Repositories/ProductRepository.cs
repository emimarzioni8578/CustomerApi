using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return context.Products.AnyAsync(product => product.Code == code, cancellationToken);
    }

    public Task<bool> AnyInCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        return context.Products.AnyAsync(product => product.CategoryId == categoryId, cancellationToken);
    }

    public void Add(Product product) => context.Products.Add(product);

    public void Remove(Product product) => context.Products.Remove(product);
}
