using Logement.Data.Enum;
using Logement.Models;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class TenantPaymentStatusViewModel
    {
        public long Id { get; set; }

        [Display(Name ="Tenant name")]
        public string TenantName { get; set; }
        public string TenantEmail { get; set; }

        [Display(Name = "Number of month to pay")]
        public int NumberOfMonthsToPay { get; set; }

        [Display(Name = "Amount remaing for rent")]
        public decimal AmountRemainingForRent { get; set; }

        [Display(Name = "Rent status")]
        public RentStatusEnum RentStatus { get; set; }
    }
}
