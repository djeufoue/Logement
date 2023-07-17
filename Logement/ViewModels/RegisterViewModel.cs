using Logement.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class RegisterViewModel
    {
     
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Confirm password does not match!")]
        public string? ConfirmPassword { get; set; }

    }
}
