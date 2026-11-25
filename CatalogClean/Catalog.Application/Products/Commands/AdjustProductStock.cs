using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using Catalog.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace Catalog.Application.Products.Commands;

public sealed record AdjustProductStockCommand(int Id, int Quantity) : ICommand;

public sealed class AdjustProductStockCommandValidator : AbstractValidator<AdjustProductStockCommand>
{
    public AdjustProductStockCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Quantity).NotEqual(0).WithMessage("La cantidad a ajustar no puede ser cero.");
    }
}

public sealed class AdjustProductStockCommandHandler(IProductRepository products, IUnitOfWork unitOfWork)
    : IRequestHandler<AdjustProductStockCommand, Result>
{
    public async Task<Result> Handle(AdjustProductStockCommand command, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(command.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(command.Id));
        }

        if (!product.CanAdjustStock(command.Quantity))
        {
            return Result.Failure(ProductErrors.InsufficientStock(product.Stock, command.Quantity));
        }

        product.AdjustStock(command.Quantity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
