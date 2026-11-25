using Catalog.Domain.Entities;
using Catalog.Domain.Exceptions;

namespace Catalog.UnitTests.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ReturnsActiveProduct()
    {
        var product = Product.Create(categoryId: 1, code: "TEC-010", name: "  Webcam HD  ", price: 35.50m, stock: 5);

        Assert.Equal("TEC-010", product.Code);
        Assert.Equal("Webcam HD", product.Name);
        Assert.Equal(35.50m, product.Price);
        Assert.Equal(5, product.Stock);
        Assert.True(product.IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("tec-010")]
    [InlineData("TEC010")]
    [InlineData("TECH-010")]
    public void Create_WithInvalidCode_ThrowsDomainException(string code)
    {
        Assert.Throws<DomainException>(() => Product.Create(1, code, "Webcam HD", 35.50m, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithNonPositivePrice_ThrowsDomainException(decimal price)
    {
        Assert.Throws<DomainException>(() => Product.Create(1, "TEC-010", "Webcam HD", price, 5));
    }

    [Fact]
    public void AdjustStock_WithNegativeQuantityWithinStock_DecreasesStock()
    {
        var product = Product.Create(1, "TEC-010", "Webcam HD", 35.50m, stock: 5);

        product.AdjustStock(-3);

        Assert.Equal(2, product.Stock);
    }

    [Fact]
    public void AdjustStock_BelowZero_ThrowsDomainExceptionAndKeepsStock()
    {
        var product = Product.Create(1, "TEC-010", "Webcam HD", 35.50m, stock: 5);

        Assert.Throws<DomainException>(() => product.AdjustStock(-6));
        Assert.Equal(5, product.Stock);
    }
}
