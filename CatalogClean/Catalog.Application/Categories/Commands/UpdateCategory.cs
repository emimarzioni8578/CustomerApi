using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Categories.Commands;

public sealed record UpdateCategoryCommand(int Id, string Name, string? Description) : ICommand;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Category.NameMaxLength);
        RuleFor(command => command.Description).MaximumLength(Category.DescriptionMaxLength);
    }
}

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(CategoryErrors.NotFound(command.Id));
        }

        if (await categories.ExistsByNameAsync(command.Name.Trim(), excludedId: command.Id, cancellationToken))
        {
            return Result.Failure(CategoryErrors.NameAlreadyExists(command.Name.Trim()));
        }

        category.Update(command.Name, command.Description);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
