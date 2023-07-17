using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [Display(Name = "nouveau mot de passe")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirmez le mot de passe")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Le mot de passe ne correspond pas!")]
        public string ConfirmNewPassword { get; set; }
        public bool IsSuccess { get; set; }
    }
}
