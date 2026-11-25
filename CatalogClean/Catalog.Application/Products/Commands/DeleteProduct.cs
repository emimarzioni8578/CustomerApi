using Catalog.Application.Abstractions.Messaging;
using Catalog.Application.Results;
using Catalog.Domain.Repositories;
using MediatR;

namespace Catalog.Application.Products.Commands;

public sealed record DeleteProductCommand(int Id) : ICommand;

public sealed class DeleteProductCommandHandler(IProductRepository products, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(command.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound(command.Id));
        }

        products.Remove(product);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
