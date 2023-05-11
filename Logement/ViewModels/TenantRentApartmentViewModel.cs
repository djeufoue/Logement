using Logement.Data.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Twilio.Types;

namespace Logement.ViewModels
{
    public class TenantRentApartmentViewModel: IValidatableObject
    {
        public long Id { get; set; }

        public long TenantId { get; set; }
        public long? CityId { get; set; }

        [DisplayName("Email")]
        public string TenantEmail { get; set; }

        [DisplayName("Phone")]
        public string? TenantPhoneNumber { get;set; }

        public long? BailId { get; set; }

        public string? Contract { get; set; }

        [Precision(14, 2)]
        [Display(Name = "Price per month after negotiation")]
        [Required]
        public decimal Price { get; set; }
  
        [Display(Name = "First payment made by the tenant")]     
        public decimal AmountPaidByTenant { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext vC)
        {
            if (AmountPaidByTenant < Price )
                yield return new ValidationResult("The first payment for a new tenant cannot be less than the price of the apartment!", new[] {"AmountPaidByTenant" });
        }

        [Precision(14, 2)]
        [DisplayName("Deposite Price")]
        public decimal DepositePrice { get; set; }

        [Display(Name = "Payment Method")]
        public PaymentMethodEnum PaymentMethod { get; set; }

        [BindProperty]
        [Display(Name = "Effective date of the contract")]
        public DateTime StartOfContract { get; set; }

        [BindProperty]
        [Display(Name = "Contract end date")]
        public DateTime? EndOfContract { get; set; }

        [NotMapped]
        [Display(Name = "Insert the lease contract")]
        public IFormFile ContractFile { get; set; }

        public ApartmentViewModel TenantApartment { get; set; }
    }
}
