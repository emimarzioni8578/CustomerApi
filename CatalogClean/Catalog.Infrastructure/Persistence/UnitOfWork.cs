using Catalog.Domain.Repositories;

namespace Catalog.Infrastructure.Persistence;

internal sealed class UnitOfWork(CatalogDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
