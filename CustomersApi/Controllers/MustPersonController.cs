using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CustomersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MustPersonController : ControllerBase
    {
        private static readonly List<MustPersonResponse> People = new();
        private readonly IValidator<MustPersonRequest> _validator;

        public MustPersonController(IValidator<MustPersonRequest> validator)
        {
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MustPersonResponse>>> GetPeople()
        {
            return Ok(People);
        }

        [HttpPost]
        public async Task<ActionResult<MustPersonResponse>> PostPerson([FromBody] MustPersonRequest request)
        {
            var validationResult = _validator.Validate(request);

            if (!validationResult.IsValid) 
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return ValidationProblem(ModelState);
            }
            return Ok();
        }
    }

    public record MustPersonRequest(
        string FirstName,
        string LastName,
        string Cuil,
        DateOnly BirthDate);

    public record MustPersonResponse(
        int Id,
        string FirstName,
        string LastName,
        string Cuil,
        DateOnly BirthDate);
}
