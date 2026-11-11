using FluentValidation;
using CustomersApi.Controllers;
namespace CustomersApi.Validators
{
    public class MustPersonRequestValidator : AbstractValidator<MustPersonRequest>
    {
        public MustPersonRequestValidator()
        {

            RuleLevelCascadeMode = CascadeMode.Continue;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(50).WithMessage("LastName cannot exceed 50 characters.");

            RuleFor(x => x.Cuil)
                .Must(IsCuilValido);

            RuleFor(x => x.BirthDate)
                .Must(NoEsFechaFutura);
        }

        private static bool IsCuilValido(string cuil) 
        {
           return CuitCuil.EsValido(cuil);
        }

        private static bool NoEsFechaFutura(DateOnly date) 
        {
            DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
            return date <= hoy;
        }
    }
}
