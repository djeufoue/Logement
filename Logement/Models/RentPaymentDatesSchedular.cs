using Logement.Data.Enum;

namespace Logement.Models
{
    public class RentPaymentDatesSchedular
    {
        public long Id { get; set; }
        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }
        
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public long ApartmentNumber { get; set; }

        public decimal AmmountSupposedToPay { get; set; }

        public DateTimeOffset NextDateToPay { get; set; }
        public RentStatusEnum RentStatus { get; set; }  
        public decimal? RemainingAmount { get; set; }
        public decimal? AmountAlreadyPaid { get; set; }
    }
}
