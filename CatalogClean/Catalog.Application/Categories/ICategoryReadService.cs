namespace Catalog.Application.Categories;

public interface ICategoryReadService
{
    Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken cancellationToken);

    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
