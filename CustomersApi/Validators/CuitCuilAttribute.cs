using System.ComponentModel.DataAnnotations;
namespace CustomersApi.Validators
{
    public class CuitCuilAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string cuitCuil && CuitCuil.EsValido(cuitCuil))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("El CUIT/CUIL no es válido.");
        }

        public sealed override bool IsValid(object? value)
        {
            bool isValid = string.IsNullOrWhiteSpace(value as string) && CuitCuil.EsValido(value as string);
            return isValid;
        }
    }
}
