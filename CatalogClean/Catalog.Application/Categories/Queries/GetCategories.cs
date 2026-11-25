using Catalog.Application.Abstractions.Messaging;
using MediatR;

namespace Catalog.Application.Categories.Queries;

public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;

public sealed class GetCategoriesQueryHandler(ICategoryReadService categories)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
    {
        return categories.ListAsync(cancellationToken);
    }
}
