using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using CustomersApi.Validators;

namespace CustomersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private static readonly List<PersonResponse> Peoples = new()
        {
            new PersonResponse(1, "Juan", "Perez", "juan.perez@example.com", "123-456-7890", "20-12345678-9", new DateOnly(1990, 1, 1), DateTime.UtcNow)
        };

        [HttpGet(Name = "GetPersons")]
        public ActionResult<IEnumerable<PersonResponse>> GetPersons()
        {
            return Ok(Peoples);
        }

        [HttpPost(Name = "PostPerson")]
        public ActionResult<PersonResponse> PostPerson([FromBody] PersonRequest personRequest)
        {
            var newPerson = new PersonResponse(
                Peoples.Count + 1,
                personRequest.FirstName,
                personRequest.LastName,
                personRequest.Email,
                personRequest.Phone,
                personRequest.Cuil,
                personRequest.BirthDate,
                DateTime.UtcNow
            );
            Peoples.Add(newPerson);
            return CreatedAtRoute(nameof(GetPersons), new { id = newPerson.Id }, newPerson);
        }

        public record PersonRequest(
            [Required][MinLength(2)][MaxLength(100)][Display(Name = "Nombre")]
        string FirstName,

            [Required][StringLength(100, MinimumLength = 2)][Display(Name = "Apellido")]
        string LastName,
            [EmailAddress]
        string Email,
            [Required][Phone]
        string? Phone,
            [Required][MaxLength(20)][CuitCuil]
        string Cuil,
            [Required][EsMayorEdad]
        DateOnly BirthDate);

        public record PersonResponse(
            int Id,
            string FirstName,
            string LastName,
            string Email,
            string? Phone,
            string Cuil,
            DateOnly BirthDate,
            DateTime CreatedAt);
    }
}
