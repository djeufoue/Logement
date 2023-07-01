using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    public class PaymentHistory
    {
        public long Id { get; set; }

        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }
        public long ApartmentNumber { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime PaidDate { get; set; }
    }
}
