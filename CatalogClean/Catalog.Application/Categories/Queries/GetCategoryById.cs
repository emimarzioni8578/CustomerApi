using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using MediatR;

namespace Catalog.Application.Categories.Queries;

public sealed record GetCategoryByIdQuery(int Id) : IQuery<Result<CategoryDto>>;

public sealed class GetCategoryByIdQueryHandler(ICategoryReadService categories)
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(query.Id, cancellationToken);

        if (category is null)
        {
            return Result<CategoryDto>.Failure(CategoryErrors.NotFound(query.Id));
        }

        return Result<CategoryDto>.Success(category);
    }
}
