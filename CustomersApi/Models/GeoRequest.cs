namespace CustomersApi.Models
{
    // Ejercicio 7: GeoRequest se valida con FluentValidation (Validators/GeoRequestValidator.cs),
    // como parte de AddressRequest. Por eso no tiene atributos.
    public class GeoRequest
    {
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public int AddressId { get; set; }
    }
}
