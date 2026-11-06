using CustomersApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomersApi.DataAccess.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Street)
                .HasMaxLength(100);
            builder.Property(a => a.Suite)
                .HasMaxLength(50);
            builder.Property(a => a.City)
                .HasMaxLength(50);
            builder.Property(a => a.Zipcode)
                .HasMaxLength(20);

            // Configuración de la relación uno a uno con Customer
            builder.HasOne(a => a.Customer)
                .WithOne(c => c.Address)
                .HasForeignKey<Address>(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Geo)
                .WithOne(g => g.Address)
                .HasForeignKey<Geo>(g => g.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
