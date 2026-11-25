using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Categories.Commands;

public sealed record CreateCategoryCommand(string Name, string? Description) : ICommand<int>;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Category.NameMaxLength);
        RuleFor(command => command.Description).MaximumLength(Category.DescriptionMaxLength);
    }
}

public sealed class CreateCategoryCommandHandler(ICategoryRepository categories, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (await categories.ExistsByNameAsync(command.Name.Trim(), excludedId: null, cancellationToken))
        {
            return Result<int>.Failure(CategoryErrors.NameAlreadyExists(command.Name.Trim()));
        }

        var category = Category.Create(command.Name, command.Description);

        categories.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(category.Id);
    }
}
