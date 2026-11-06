namespace CustomersApi.Entities
{
    public class Geo
    {
        public int Id { get; set; }
        public string? Lat { get; set; }
        public string? Lng { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }
    }
}
