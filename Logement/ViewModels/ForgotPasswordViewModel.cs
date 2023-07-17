using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress, Display(Name = "Adresse email")]
        public string Email { get; set; } = "";
        public bool EmailSent { get; set; }
    }
}
