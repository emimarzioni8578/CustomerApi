using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Categories.Commands;

public sealed record DeleteCategoryCommand(int Id) : ICommand;

public sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categories,
    IProductRepository products,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(command.Id));
        }

        if (await products.AnyInCategoryAsync(command.Id, cancellationToken))
        {
            return Result.Failure(CategoryErrors.HasProducts(category.Name));
        }

        categories.Remove(category);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
