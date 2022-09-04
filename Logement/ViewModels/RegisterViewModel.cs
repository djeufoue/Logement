using Logement.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class RegisterViewModel
    {
     
        public string TenantFirstName { get; set; }

        public string TenantLastName { get; set; }

        public string JobTitle { get; set; }

        public MaritalStatusEnum? MaritalStatus { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Confirm password does not match!")]
        public string? ConfirmPassword { get; set; }

    }
}
