using System.ComponentModel.DataAnnotations;
namespace CustomersApi.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class , AllowMultiple = false)]
    public class EsMayorEdadAttribute : ValidationAttribute
    {
        public sealed override bool IsValid(object? value)
        {
            //TO-DO
            return base.IsValid(value);
        }
    }
}
