using Catalog.Domain.Exceptions;

namespace Catalog.Domain.Entities;

public class Category
{
    public const int NameMaxLength = 80;
    public const int DescriptionMaxLength = 300;

    private Category()
    {
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public static Category Create(string name, string? description)
    {
        var category = new Category();
        category.Update(name, description);
        return category;
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("El nombre de la categoría es obligatorio.");
        }

        if (name.Trim().Length > NameMaxLength)
        {
            throw new DomainException($"El nombre de la categoría no puede superar los {NameMaxLength} caracteres.");
        }

        var normalizedDescription = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

        if (normalizedDescription?.Length > DescriptionMaxLength)
        {
            throw new DomainException($"La descripción no puede superar los {DescriptionMaxLength} caracteres.");
        }

        Name = name.Trim();
        Description = normalizedDescription;
    }
}
