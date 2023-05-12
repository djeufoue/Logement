using Logement.Data.Enum;
using Logement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Logement.ViewModels
{
    public class CityMemberViewModel
    {
        public long Id { get; set; }
        public ApartmentViewModel AppartmentMember { get; set; } = new ApartmentViewModel();

        public long? TenantId { get; set; }

        [Precision(14, 2)]
        [Display(Name = "Prix par mois")]
        [Required]
        public decimal Price { get; set; }

        public decimal AmountPaidByTenant { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext vC)
        {
            if (AmountPaidByTenant < Price)
                yield return new ValidationResult("The first payment for a new tenant cannot be less than the price of the apartment!", new[] { "AmountPaidByTenant" });
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
    }
}
