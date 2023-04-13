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

        //to be added automatically in the code
        public long TenantId { get; set; }

        /// <summary>
        /// The lessor must provide a valid Email
        /// Which is going to be use find the user that he just 
        /// add as tenant
        /// </summary>
        [DisplayName("Email")]
        public string TenantEmail { get; set; }

        [DisplayName("Phone")]

        public string? TenantPhoneNumber { get;set; }

        //to be added automatically in the code(the id of the new FileModel)
        public long? BailId { get; set; }

        public string? Contract { get; set; }

        //to be added automatically by the Lessor
        /// <summary>
        /// Price after nagociations 
        /// </summary>
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

        public ApartmentViewModel TenantApartment { get; set; }
    }
}
