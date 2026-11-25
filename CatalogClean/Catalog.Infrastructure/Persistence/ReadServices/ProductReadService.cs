using Catalog.Application.Products;
using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Persistence.ReadServices;

internal sealed class ProductReadService(CatalogDbContext context) : IProductReadService
{
    public async Task<IReadOnlyList<ProductDto>> ListAsync(int? categoryId, bool onlyActive, CancellationToken cancellationToken)
    {
        var products = context.Products.AsNoTracking();

        if (categoryId is not null)
        {
            products = products.Where(product => product.CategoryId == categoryId);
        }

        if (onlyActive)
        {
            products = products.Where(product => product.IsActive);
        }

        return await ToDto(products).ToListAsync(cancellationToken);
    }

    public Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var products = context.Products.AsNoTracking().Where(product => product.Id == id);

        return ToDto(products).FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<ProductDto> ToDto(IQueryable<Product> products)
    {
        return from product in products
               join category in context.Categories.AsNoTracking() on product.CategoryId equals category.Id
               orderby product.Id
               select new ProductDto(
                   product.Id,
                   product.Code,
                   product.Name,
                   product.Price,
                   product.Stock,
                   product.IsActive,
                   product.CreatedAt,
                   category.Id,
                   category.Name);
    }
}
