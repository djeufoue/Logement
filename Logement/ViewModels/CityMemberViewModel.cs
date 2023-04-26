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
        public CityViewModel City { get; set; }
        public ApartmentViewModel AppartmentMember { get; set; } = new ApartmentViewModel();
        public CityMemberRoleEnum Role { get; set; }

        [DisplayName("Email")]
        public string TenantEmail { get; set; }

        [DisplayName("Phone")]

        public string? TenantPhoneNumber { get; set; }

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

        //to be added automatically by the Lessor
        /// <summary>
        /// La caution qui a ete paye
        /// </summary>
        [Precision(14, 2)]
        [DisplayName("Deposite Price")]
        public decimal DepositePrice { get; set; }


        //to be added automatically by the Lessor
        /// <summary>
        /// There are tenants who pay by the month and others 
        /// after every two months or even by the year; 
        /// it depends on the arrangement he has with his owner
        /// </summary>
        [Display(Name = "Payment Method")]
        public PaymentMethodEnum PaymentMethod { get; set; }

        //to be set using the Now.Utc time
        /// <summary>
        /// It is on this date that the tenant begins to pay his rent.
        /// </summary>
        [BindProperty]
        [Display(Name = "Effective date of the contract")]
        public DateTime StartOfContract { get; set; }


        /// <summary>
        /// This marks the end of the contract that the tenant had signed 
        /// with the lessor, but does not mark the end of the payment of 
        /// the rent: "it is possible that the tenant still owes money to 
        /// the lessor after the end of his contract"
        /// </summary>
        [BindProperty]
        [Display(Name = "Contract end date")]
        public DateTime? EndOfContract { get; set; }

        [NotMapped]
        [Display(Name = "Insert the lease contract")]
        public IFormFile ContractFile { get; set; }
    }
}
