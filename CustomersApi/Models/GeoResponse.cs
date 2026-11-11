namespace CustomersApi.Models
{
    public class GeoResponse
    {
        public int Id { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public int AddressId { get; set; }
    }
}
