using CustomersApi.Mappers;
using CustomersApi.Models;
using CustomersApi.Services;
using CustomersApi.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CustomersApi.Controllers
{
    // Controller: recibe el HTTP, mapea DTOs <-> entidades y traduce excepciones a códigos HTTP.
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersService _service;

        public CustomersController(ICustomersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetCustomers()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers.ToDtoList());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerResponse>> GetCustomer([FromRoute] int id)
        {
            try
            {
                var customer = await _service.GetByIdAsync(id);
                return Ok(customer.ToDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CustomerResponse>> PostCustomer([FromBody] CustomerRequest request)
        {
            try
            {
                var customer = await _service.CreateAsync(request.ToEntity());
                return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer.ToDto());
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutCustomer([FromRoute] int id, [FromBody] CustomerRequest request)
        {
            try
            {
                await _service.UpdateAsync(id, request.ToEntity());
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DuplicateEmailException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
