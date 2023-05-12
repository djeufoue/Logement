using Logement.Models;
using System.ComponentModel;

namespace Logement.ViewModels
{
    public class RentPaymentDatesSchedularViewModel
    {
        public long Id { get; set; }

        [DisplayName("Tenant Email")]
        public string TenantFullName { get; set; }

        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }

        [DisplayName("Amount to be paid [FCFA]")]
        public decimal AmmountSupposedToPay { get; set; }

        [DisplayName("Already been paid")]
        public string IsRentPaidForThisDate { get; set; }

        [DisplayName("Date to pay")]
        public string NextDateToPay { get; set; }
    }
}
