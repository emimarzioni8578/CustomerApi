namespace CustomersApi.Models
{
    // Ejercicio 7: AddressRequest se valida con FluentValidation (Validators/AddressRequestValidator.cs).
    // Por eso no tiene atributos: un DTO, una estrategia.
    // (En el ejercicio 4, Zipcode tenía [CodigoPostalArgentino]; el validador reutiliza su Patron.)
    public class AddressRequest
    {
        public string? Street { get; set; }
        public string? Suite { get; set; }
        public string? City { get; set; }
        public string? Zipcode { get; set; }
        public int CustomerId { get; set; }
        public GeoRequest? Geo { get; set; }
    }
}
