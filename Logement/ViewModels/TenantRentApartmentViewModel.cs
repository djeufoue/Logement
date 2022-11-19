using Logement.Data.Enum;
using Logement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logement.ViewModels
{
    public class TenantRentApartmentViewModel
    {
        public long Id { get; set; }

        //to be added automatically in the code But the apartment must be choose by the lessor
        public long ApartmentId { get; set; }

        //to be added automatically in the code
        public long TenantId { get; set; }

        /// <summary>
        /// The lessor must provide a valid Email
        /// Which is going to be use find the user that he just 
        /// add as tenant
        /// </summary>
        [Required]
        public string TenantEmail { get; set; }

        //to be added automatically in the code(the id of the new FileModel)
        public long BailId { get; set; }

        public string? Contract { get; set; }

        //to be added automatically by the Lessor
        /// <summary>
        /// Price after nagociations 
        /// </summary>
        [Precision(14, 2)]
        [Display(Name = "Price after nagociations")]
        [Required]
        public decimal Price { get; set; }

        //to be added automatically by the Lessor
        /// <summary>
        /// La caution qui a ete paye
        /// </summary>
        [Precision(14, 2)]
        public decimal DepositePrice { get; set; }

        [Display(Name = "Amount given by the tenant")]
        public decimal AmountPaidByTenant { get; set; }

        /// <summary>
        /// Need to be calculate automaticaly base on the apartment price
        /// the amount that was paid by the tenant and also the amount of months before set.
        /// </summary>
        public int NumberOfMonthsToPay { get; set; }

        /// <summary>
        /// Need to be calculate automaticaly base on the apartment price
        /// the amount that was paid by the tenant and also the amount of months before set.
        /// </summary>
        public decimal AmountRemainingForRent { get; set; }

        public decimal AmountPaidInAdvance { get; set; }

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
