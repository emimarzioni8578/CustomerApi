using CustomersApi.Entities;

namespace CustomersApi.DataAccess.Repositories
{
    // Repositorio: única capa que habla con EF Core / la base de datos.
    public interface ICustomersRepository
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null);
        Task<Customer> AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}
