using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class PaymentHistoryViewModel
    {
        public long Id { get; set; }

        public long TenantId { get; set; }

        [Display(Name = "Full name")]
        public string TenantFullName { get; set; }
        //String because the 

        [Display(Name = "Number of month paid")]
        public string NumberOfMonthPaid { get; set; }

        [Display(Name = "Amount paid")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Paid date")]
        public string PaidDate { get; set; }
    }
}
