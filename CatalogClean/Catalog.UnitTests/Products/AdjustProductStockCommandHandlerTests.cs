using Catalog.Application.Products.Commands;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.UnitTests.Fakes;

namespace Catalog.UnitTests.Products;

public class AdjustProductStockCommandHandlerTests
{
    private readonly InMemoryProductRepository _products = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly AdjustProductStockCommandHandler _handler;

    public AdjustProductStockCommandHandlerTests()
    {
        _products.Add(Product.Create(1, "TEC-010", "Webcam HD", 35.50m, stock: 5));
        _handler = new AdjustProductStockCommandHandler(_products, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithQuantityWithinStock_UpdatesStock()
    {
        var result = await _handler.Handle(new AdjustProductStockCommand(Id: 1, Quantity: -3), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, _products.Items[0].Stock);
        Assert.Equal(1, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenStockIsInsufficient_ReturnsConflictErrorAndKeepsStock()
    {
        var result = await _handler.Handle(new AdjustProductStockCommand(Id: 1, Quantity: -6), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error!.Type);
        Assert.Equal(5, _products.Items[0].Stock);
        Assert.Equal(0, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNotFoundError()
    {
        var result = await _handler.Handle(new AdjustProductStockCommand(Id: 99, Quantity: 1), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }
}
