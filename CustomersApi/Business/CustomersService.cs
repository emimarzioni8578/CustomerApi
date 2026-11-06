using CustomersApi.DataAccess.Repositories;
using CustomersApi.Entities;
using CustomersApi.Services.Exceptions;

namespace CustomersApi.Business
{
    public class CustomersService : ICustomersService
    {
        private readonly ICustomersRepository _repository;

        public CustomersService(ICustomersRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);

            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with ID {id} not found.");
            }

            return customer;
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            await EnsureEmailIsAvailableAsync(customer.Email);

            return await _repository.AddAsync(customer);
        }

        public async Task<Customer> UpdateAsync(int id, Customer customer)
        {
            var existingCustomer = await GetByIdAsync(id);

            await EnsureEmailIsAvailableAsync(customer.Email);

            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;

            await _repository.UpdateAsync(existingCustomer);

            return existingCustomer;
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await GetByIdAsync(id);

            await _repository.DeleteAsync(customer);
        }

        // Regla de negocio: no puede haber dos clientes con el mismo email.
        private async Task EnsureEmailIsAvailableAsync(string email)
        {
            if (await _repository.EmailExistsAsync(email))
            {
                throw new DuplicateEmailException($"A customer with email '{email}' already exists.");
            }
        }
    }
}
