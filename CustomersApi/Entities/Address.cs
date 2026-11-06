namespace CustomersApi.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? Suite { get; set; }
        public string? City { get; set; }
        public string? Zipcode { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public Geo? Geo { get; set; }
    }
}
