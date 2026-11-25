using Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categorias");

        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).HasColumnName("CategoriaId");

        builder.Property(category => category.Name)
            .HasColumnName("Nombre")
            .HasMaxLength(Category.NameMaxLength);

        builder.Property(category => category.Description)
            .HasColumnName("Descripcion")
            .HasMaxLength(Category.DescriptionMaxLength);

        builder.HasIndex(category => category.Name, "UQ_Categorias_Nombre").IsUnique();
    }
}
