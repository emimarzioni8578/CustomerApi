using Catalog.Application.Behaviors;
using Catalog.Application.Products.Commands;
using Catalog.Application.Results;

namespace Catalog.UnitTests.Behaviors;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateProductCommand, Result<int>> _behavior =
        new([new CreateProductCommandValidator()]);

    private bool _handlerWasCalled;

    [Fact]
    public async Task Handle_WithInvalidRequest_ReturnsValidationErrorWithoutCallingHandler()
    {
        var command = new CreateProductCommand(CategoryId: 1, Code: "tec10", Name: "Webcam HD", Price: 0, Stock: 5);

        var result = await _behavior.Handle(command, Handler, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
        Assert.Equal(["Code", "Price"], result.Error.Fields!.Keys.Order());
        Assert.False(_handlerWasCalled);
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsHandler()
    {
        var command = new CreateProductCommand(CategoryId: 1, Code: "TEC-010", Name: "Webcam HD", Price: 35.50m, Stock: 5);

        var result = await _behavior.Handle(command, Handler, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.True(_handlerWasCalled);
    }

    private Task<Result<int>> Handler(CancellationToken cancellationToken = default)
    {
        _handlerWasCalled = true;
        return Task.FromResult(Result<int>.Success(42));
    }
}
