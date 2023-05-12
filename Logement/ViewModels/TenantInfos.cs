using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class TenantInfos
    {
        [Display(Name = "Apartment number")]
        public int ApartmentNumber { get; set; }
        public long Id { get; set; }

        [Display(Name ="Full name")]
        public string? TenantFullName { get; set; }
        public long TenantId { get; set; }

        [Display(Name = "Phone number")]
        public string? TenantPhoneNumber { get; set; }

        [Display(Name = "Apartement price")]
        public decimal? ApartementPrice { get; set; }

        [Precision(14, 2)]
        [DisplayName("Deposite Price")]
        public decimal? DepositePrice { get; set; }

        [BindProperty]
        [Display(Name = "Contract end date")]
        public DateTime? EndOfContract { get; set; }
    }
}
