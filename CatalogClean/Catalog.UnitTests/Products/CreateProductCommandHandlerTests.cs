using Catalog.Application.Products.Commands;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.UnitTests.Fakes;

namespace Catalog.UnitTests.Products;

public class CreateProductCommandHandlerTests
{
    private readonly InMemoryProductRepository _products = new();
    private readonly InMemoryCategoryRepository _categories = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _categories.Add(Category.Create("Tecnología", null));
        _handler = new CreateProductCommandHandler(_products, _categories, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProductAndReturnsItsId()
    {
        var command = new CreateProductCommand(CategoryId: 1, Code: "TEC-010", Name: "Webcam HD", Price: 35.50m, Stock: 5);

        var result = await _handler.Handle(command, CancellationToken.None);

        var product = Assert.Single(_products.Items);
        Assert.True(result.IsSuccess);
        Assert.Equal(product.Id, result.Value);
        Assert.Equal("TEC-010", product.Code);
        Assert.Equal(1, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNotFoundError()
    {
        var command = new CreateProductCommand(CategoryId: 99, Code: "TEC-010", Name: "Webcam HD", Price: 35.50m, Stock: 5);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
        Assert.Empty(_products.Items);
        Assert.Equal(0, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenCodeAlreadyExists_ReturnsConflictError()
    {
        _products.Add(Product.Create(1, "TEC-010", "Webcam HD", 35.50m, 5));
        var command = new CreateProductCommand(CategoryId: 1, Code: "TEC-010", Name: "Otra webcam", Price: 20m, Stock: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error!.Type);
        Assert.Single(_products.Items);
        Assert.Equal(0, _unitOfWork.SaveChangesCalls);
    }
}
