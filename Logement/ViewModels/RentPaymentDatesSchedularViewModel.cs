using Logement.Data.Enum;
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

        [DisplayName("Remaining amount [FCFA]")]
        public decimal? RemainingAmount { get; set; }

        [DisplayName("Amount already paid [FCFA]")]
        public decimal? AmountAlreadyPaid { get; set; }

        [DisplayName("Date to pay")]
        public string ExpectedDateToPay { get; set; }

        public RentStatusEnum RentStatus { get; set; }

    }
}
