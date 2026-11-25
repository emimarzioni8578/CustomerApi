using System.Text.RegularExpressions;
using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public partial class Product
{
    public const int NameMaxLength = 120;
    public const string CodePattern = "^[A-Z]{3}-[0-9]{3}$";

    private Product()
    {
    }

    public int Id { get; private set; }

    public int CategoryId { get; private set; }

    public string Code { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Product Create(int categoryId, string code, string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(code) || !CodeRegex().IsMatch(code))
        {
            throw new DomainException("El código del producto debe tener el formato AAA-000 (por ejemplo, TEC-001).");
        }

        if (stock < 0)
        {
            throw new DomainException("El stock inicial no puede ser negativo.");
        }

        var product = new Product
        {
            Code = code,
            Stock = stock,
            IsActive = true
        };
        product.UpdateDetails(categoryId, name, price);

        return product;
    }

    public void UpdateDetails(int categoryId, string name, decimal price)
    {
        if (categoryId <= 0)
        {
            throw new DomainException("El producto debe pertenecer a una categoría.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre del producto es obligatorio.");
        }

        if (name.Trim().Length > NameMaxLength)
        {
            throw new DomainException($"El nombre del producto no puede superar los {NameMaxLength} caracteres.");
        }

        if (price <= 0)
        {
            throw new DomainException("El precio del producto debe ser mayor que cero.");
        }

        CategoryId = categoryId;
        Name = name.Trim();
        Price = price;
    }

    public bool CanAdjustStock(int quantity) => Stock + quantity >= 0;

    public void AdjustStock(int quantity)
    {
        if (!CanAdjustStock(quantity))
        {
            throw new DomainException($"Stock insuficiente: hay {Stock} unidades y se intentó descontar {-quantity}.");
        }

        Stock += quantity;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    [GeneratedRegex(CodePattern)]
    private static partial Regex CodeRegex();
}
