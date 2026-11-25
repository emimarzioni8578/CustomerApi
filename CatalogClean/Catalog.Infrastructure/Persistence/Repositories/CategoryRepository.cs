using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Categories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return context.Categories.AnyAsync(category => category.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken)
    {
        return context.Categories.AnyAsync(
            category => category.Name == name && (excludedId == null || category.Id != excludedId),
            cancellationToken);
    }

    public void Add(Category category) => context.Categories.Add(category);

    public void Remove(Category category) => context.Categories.Remove(category);
}
