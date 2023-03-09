using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    
    public class PaymentHistory
    {
        public long Id { get; set; }

        public string TenantEmail { get; set; }
        //String because the 
        public string NunberOfMonthPaid { get; set; }

        public decimal AmountPaid { get; set; }

        public DateTime PaidDate { get; set; }
    }
}
