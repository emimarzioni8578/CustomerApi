using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Productos");

        builder.HasKey(product => product.Id);
        builder.Property(product => product.Id).HasColumnName("ProductoId");

        builder.Property(product => product.CategoryId).HasColumnName("CategoriaId");

        builder.Property(product => product.Code)
            .HasColumnName("Codigo")
            .HasMaxLength(20)
            .IsUnicode(false);

        builder.Property(product => product.Name)
            .HasColumnName("Nombre")
            .HasMaxLength(Product.NameMaxLength);

        builder.Property(product => product.Price)
            .HasColumnName("Precio")
            .HasColumnType("decimal(10, 2)");

        builder.Property(product => product.Stock).HasColumnName("Stock");

        builder.Property(product => product.IsActive).HasColumnName("Activo");

        builder.Property(product => product.CreatedAt)
            .HasColumnName("FechaAlta")
            .HasPrecision(0)
            .HasDefaultValueSql("(sysutcdatetime())");

        builder.HasIndex(product => product.Code, "UQ_Productos_Codigo").IsUnique();

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Productos_Categorias");
    }
}
