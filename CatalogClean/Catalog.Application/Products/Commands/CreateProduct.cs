using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Categories;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Products.Commands;

public sealed record CreateProductCommand(int CategoryId, string Code, string Name, decimal Price, int Stock)
    : ICommand<int>;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.CategoryId).GreaterThan(0);
        RuleFor(command => command.Code)
            .NotEmpty()
            .Matches(Product.CodePattern).WithMessage("El código debe tener el formato AAA-000 (por ejemplo, TEC-001).");
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Product.NameMaxLength);
        RuleFor(command => command.Price).GreaterThan(0);
        RuleFor(command => command.Stock).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateProductCommandHandler(
    IProductRepository products,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (!await categories.ExistsAsync(command.CategoryId, cancellationToken))
        {
            return Result<int>.Failure(CategoryErrors.NotFound(command.CategoryId));
        }

        if (await products.ExistsByCodeAsync(command.Code, cancellationToken))
        {
            return Result<int>.Failure(ProductErrors.CodeAlreadyExists(command.Code));
        }

        var product = Product.Create(command.CategoryId, command.Code, command.Name, command.Price, command.Stock);

        products.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(product.Id);
    }
}
