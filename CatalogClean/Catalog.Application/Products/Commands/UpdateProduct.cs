using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Categories;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Products.Commands;

public sealed record UpdateProductCommand(int Id, int CategoryId, string Name, decimal Price) : ICommand;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.CategoryId).GreaterThan(0);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Product.NameMaxLength);
        RuleFor(command => command.Price).GreaterThan(0);
    }
}

public sealed class UpdateProductCommandHandler(
    IProductRepository products,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, Result>
{
    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(command.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(command.Id));
        }

        if (!await categories.ExistsAsync(command.CategoryId, cancellationToken))
        {
            return Result.Failure(CategoryErrors.NotFound(command.CategoryId));
        }

        product.UpdateDetails(command.CategoryId, command.Name, command.Price);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
