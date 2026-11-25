using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken);

    void Add(Category category);

    void Remove(Category category);
}
