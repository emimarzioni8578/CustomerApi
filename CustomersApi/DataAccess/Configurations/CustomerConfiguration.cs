using CustomersApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomersApi.DataAccess.Configurations
{
    // Fluent API: define cómo se mapea la entidad a la tabla (largos, índices, datos iniciales).
    // Cualquier cambio aquí requiere una nueva migración.
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");

            builder.HasKey(customer => customer.Id);

            builder.Property(customer => customer.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(customer => customer.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(customer => customer.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(customer => customer.Email)
                .IsUnique();

            builder.Property(customer => customer.Phone)
                .HasMaxLength(30);

            builder.Property(customer => customer.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Datos semilla: se insertan con la migración. Los valores deben ser fijos (nada de DateTime.Now).
            builder.HasData(
                new Customer { Id = 1, FirstName = "Ada", LastName = "Lovelace", Email = "ada@example.com", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Customer { Id = 2, FirstName = "Alan", LastName = "Turing", Email = "alan@example.com", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Customer { Id = 3, FirstName = "Grace", LastName = "Hopper", Email = "grace@example.com", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
        }
    }
}
