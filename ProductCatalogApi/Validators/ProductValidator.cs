using FluentValidation;
using ProductCatalogApi.Entities;

namespace ProductCatalogApi.Validators
{
    public class ProductValidator : AbstractValidator<Producto>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Codigo).NotEmpty().WithMessage("El código del producto es obligatorio.");
            RuleFor(p => p.Nombre).NotEmpty().WithMessage("El nombre del producto es obligatorio.");
            RuleFor(p => p.Precio).GreaterThan(0).WithMessage("El precio del producto debe ser mayor que cero.");
            RuleFor(p => p.Stock).GreaterThanOrEqualTo(0).WithMessage("El stock del producto no puede ser negativo.");
            RuleFor(p => p.CategoriaId).GreaterThan(0).WithMessage("La categoría del producto es obligatoria.");
        }
    }
}
