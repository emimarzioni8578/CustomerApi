using CustomersApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomersApi.DataAccess.Repositories
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly CustomersDbContext _dbContext;

        public CustomersRepository(CustomersDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            // AsNoTracking: consulta de solo lectura, EF no necesita seguir cambios de estas entidades.
            return await _dbContext.Customers
                .AsNoTracking()
                .OrderBy(customer => customer.Id)
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _dbContext.Customers.FindAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
        {
            var query = _dbContext.Customers.Where(customer => customer.Email == email);

            if (excludeCustomerId.HasValue)
            {
                query = query.Where(customer => customer.Id != excludeCustomerId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync();

            // Después de SaveChanges, EF completa Id y CreatedAt con los valores generados por la base.
            return customer;
        }

        public async Task UpdateAsync(Customer customer)
        {
            _dbContext.Customers.Update(customer);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Customer customer)
        {
            _dbContext.Customers.Remove(customer);
            await _dbContext.SaveChangesAsync();
        }
    }
}
