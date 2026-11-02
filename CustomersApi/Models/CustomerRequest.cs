using System.ComponentModel.DataAnnotations;

namespace CustomersApi.Models
{
    // DTO de entrada (POST / PUT). Las validaciones se ejecutan solas gracias a [ApiController].
    public class CustomerRequest
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(30)]
        public string? Phone { get; set; }
    }
}
