using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class TenantInfos
    {
        public long Id { get; set; }
        public string? TenantFullName { get; set; }
        public string? TenantEmail { get; set; }
        public string? TenantPhoneNumber { get; set; }
        public decimal? ApartementPrice { get; set; }

        [Precision(14, 2)]
        [DisplayName("Deposite Price")]
        public decimal? DepositePrice { get; set; }

        [BindProperty]
        [Display(Name = "Contract end date")]
        public DateTime? EndOfContract { get; set; }
    }
}
