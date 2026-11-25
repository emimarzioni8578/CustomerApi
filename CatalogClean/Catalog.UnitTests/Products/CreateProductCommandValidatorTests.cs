using Catalog.Application.Products.Commands;

namespace Catalog.UnitTests.Products;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_HasNoErrors()
    {
        var command = new CreateProductCommand(CategoryId: 1, Code: "TEC-010", Name: "Webcam HD", Price: 35.50m, Stock: 5);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvalidCommand_ReportsEveryInvalidProperty()
    {
        var command = new CreateProductCommand(CategoryId: 0, Code: "tec10", Name: "", Price: 0, Stock: -1);

        var result = _validator.Validate(command);

        var invalidProperties = result.Errors.Select(error => error.PropertyName).Distinct().Order().ToArray();
        Assert.Equal(["CategoryId", "Code", "Name", "Price", "Stock"], invalidProperties);
    }
}
