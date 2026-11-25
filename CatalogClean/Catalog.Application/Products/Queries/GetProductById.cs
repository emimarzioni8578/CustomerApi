using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using MediatR;

namespace Catalog.Application.Products.Queries;

public sealed record GetProductByIdQuery(int Id) : IQuery<Result<ProductDto>>;

public sealed class GetProductByIdQueryHandler(IProductReadService products)
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(query.Id, cancellationToken);

        if (product is null)
        {
            return Result<ProductDto>.Failure(ProductErrors.NotFound(query.Id));
        }

        return Result<ProductDto>.Success(product);
    }
}
