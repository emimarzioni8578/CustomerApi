using CustomersApi.Entities;
using CustomersApi.Models;

namespace CustomersApi.Mappers
{
    // Convierte entre DTOs (Models) y entidades (Entities).
    public static class CustomerMapper
    {
        public static Customer ToEntity(this CustomerRequest request)
        {
            return new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone
            };
        }

        public static CustomerResponse ToDto(this Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedAt = customer.CreatedAt
            };
        }

        public static IEnumerable<CustomerResponse> ToDtoList(this IEnumerable<Customer> customers)
        {
            return customers.Select(customer => customer.ToDto());
        }
    }
}
