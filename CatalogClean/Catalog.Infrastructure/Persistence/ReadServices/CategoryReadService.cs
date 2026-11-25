using Catalog.Application.Categories;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.ReadServices;

internal sealed class CategoryReadService(CatalogDbContext context) : ICategoryReadService
{
    public async Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken cancellationToken)
    {
        return await context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDto(category.Id, category.Name, category.Description))
            .ToListAsync(cancellationToken);
    }

    public Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return context.Categories
            .AsNoTracking()
            .Where(category => category.Id == id)
            .Select(category => new CategoryDto(category.Id, category.Name, category.Description))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
