using Catalog.Application.Categories.Commands;
using Catalog.Application.Results;
using Catalog.Domain.Entities;
using Catalog.UnitTests.Fakes;

namespace Catalog.UnitTests.Categories;

public class DeleteCategoryCommandHandlerTests
{
    private readonly InMemoryProductRepository _products = new();
    private readonly InMemoryCategoryRepository _categories = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _categories.Add(Category.Create("Tecnología", null));
        _handler = new DeleteCategoryCommandHandler(_categories, _products, _unitOfWork);
    }

    [Fact]
    public async Task Handle_WhenCategoryHasNoProducts_RemovesCategory()
    {
        var result = await _handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(_categories.Items);
        Assert.Equal(1, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenCategoryHasProducts_ReturnsConflictError()
    {
        _products.Add(Product.Create(1, "TEC-010", "Webcam HD", 35.50m, 5));

        var result = await _handler.Handle(new DeleteCategoryCommand(1), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error!.Type);
        Assert.Single(_categories.Items);
        Assert.Equal(0, _unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNotFoundError()
    {
        var result = await _handler.Handle(new DeleteCategoryCommand(99), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error!.Type);
    }
}
