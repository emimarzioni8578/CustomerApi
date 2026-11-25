using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;

namespace Catalog.UnitTests.Fakes;

internal sealed class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> _categories = [];

    public IReadOnlyList<Category> Items => _categories;

    public Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categories.FirstOrDefault(category => category.Id == id));
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categories.Any(category => category.Id == id));
    }

    public Task<bool> ExistsByNameAsync(string name, int? excludedId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_categories.Any(category =>
            string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase) && category.Id != excludedId));
    }

    public void Add(Category category)
    {
        GeneratedId.Assign(category, _categories.Count + 1);
        _categories.Add(category);
    }

    public void Remove(Category category) => _categories.Remove(category);
}
