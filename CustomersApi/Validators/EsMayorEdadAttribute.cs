using System.ComponentModel.DataAnnotations;
namespace CustomersApi.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.All , AllowMultiple = true)]
    public class EsMayorEdadAttribute : ValidationAttribute
    {
        public sealed override bool IsValid(object? value)
        {

            if (value is DateTime fechaNacimiento)
            {
                var edad = DateTime.Today.Year - fechaNacimiento.Year;
                if (fechaNacimiento > DateTime.Today.AddYears(-edad))
                {
                    edad--;
                }
                // return edad >= EdadMinima;
                return base.IsValid(value);
            }
            return false;

        }
    }
}

