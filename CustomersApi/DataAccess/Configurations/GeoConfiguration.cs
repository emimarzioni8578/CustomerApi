using CustomersApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomersApi.DataAccess.Configurations
{
    public class GeoConfiguration : IEntityTypeConfiguration<Geo>
    {
        public void Configure(EntityTypeBuilder<Geo> builder)
        {
            builder.ToTable("Geo");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Lat)
                .HasMaxLength(50);
            builder.Property(g => g.Lng)
                .HasMaxLength(50);

            // Configuración de la relación uno a uno con Address
            builder.HasOne(g => g.Address)
                .WithOne(a => a.Geo)
                .HasForeignKey<Geo>(g => g.AddressId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
