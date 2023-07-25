using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string JobTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
    }
}
