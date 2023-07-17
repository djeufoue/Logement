using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace Logement.ViewModels
{
    public class PaymentHistoryViewModel
    {
        public long Id { get; set; }

        public long TenantId { get; set; }
       /* public long CityId { get; set; }
        public string? CityName { get; set; }*/

        [Display(Name = "Apartment Number")]
        public long ApartmentNumber { get; set; }

        [Display(Name = "Full name")]
        public string TenantFullName { get; set; }

        [Display(Name = "Amount paid")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "Paid date")]
        public string PaidDate { get; set; }
    }
}
