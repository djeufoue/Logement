using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Email Address is required")]
        [EmailAddress]
        [Display(Name ="Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; } = true;
    }
}
