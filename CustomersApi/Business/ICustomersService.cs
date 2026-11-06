using CustomersApi.Entities;

namespace CustomersApi.Business
{
    // Servicio: contiene las reglas de negocio. No sabe nada de HTTP ni de EF Core.
    public interface ICustomersService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer> UpdateAsync(int id, Customer customer);
        Task DeleteAsync(int id);
    }
}
