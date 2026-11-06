using CustomersApi.DataAccess.Configurations;
using CustomersApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomersApi.DataAccess
{
    // El DbContext es la "puerta" a la base de datos: cada DbSet es una tabla.
    public class CustomersDbContext : DbContext
    {
        public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Geo> Geos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new GeoConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
