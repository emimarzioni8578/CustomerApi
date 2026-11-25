using Catalog.Application.Abstractions.Messaging;
using MediatR;

namespace Catalog.Application.Products.Queries;

public sealed record GetProductsQuery(int? CategoryId, bool OnlyActive) : IQuery<IReadOnlyList<ProductDto>>;

public sealed class GetProductsQueryHandler(IProductReadService products)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        return products.ListAsync(query.CategoryId, query.OnlyActive, cancellationToken);
    }
}
