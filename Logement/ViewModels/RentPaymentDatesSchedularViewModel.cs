using Logement.Data.Enum;
using Logement.Models;
using Microsoft.EntityFrameworkCore.Storage;
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

        /*public long CityId { get; set; }    
        public string? CityName { get; set; }*/

        [DisplayName("Apartment Number")]
        public long ApartmentNumber { get; set; }

        public long CityId { get; set; }

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
