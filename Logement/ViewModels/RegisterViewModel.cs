using Logement.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class RegisterViewModel: IValidatableObject
    {
     
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string JobTitle { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [RegularExpression(@"^[\+\d]?(?:[\d-.\s()]*)$", ErrorMessage = "Your phone number must start with + and followed by your country code")]
        public string? PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext vC)
        {
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(PhoneNumber))
                yield return new ValidationResult("Choose either email, phone number, or both!", new[] { "Email", "PhoneNumber" });
        }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Confirm password does not match!")]
        public string? ConfirmPassword { get; set; }

    }
}
