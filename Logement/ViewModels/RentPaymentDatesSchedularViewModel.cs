using System.ComponentModel;

namespace Logement.ViewModels
{
    public class RentPaymentDatesSchedularViewModel
    {
        public long Id { get; set; }

        [DisplayName("Tenant Email")]
        public string TenantEmail { get; set; }

        [DisplayName("Amount to be paid")]
        public decimal AmmountSupposedToPay { get; set; }

        [DisplayName("Already been paid")]
        public string IsRentPaidForThisDate { get; set; }

        [DisplayName("Date to pay")]
        public string NextDateToPay { get; set; }
    }
}
