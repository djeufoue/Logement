using Logement.Data.Enum;

namespace Logement.Models
{
    //Fill this table after we add a tenant, and conditionally per month or per year and so on..
    public class TenantPaymentStatus
    {
        public long Id { get; set; }
        
        public string TenantEmail { get; set; }

        public int NumberOfMonthsToPay { get; set; }

        public decimal AmountRemainingForRent { get; set; }

        public RentStatusEnum RentStatus { get; set; }
    }
}
