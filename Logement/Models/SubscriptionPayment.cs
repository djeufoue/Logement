using System.ComponentModel.DataAnnotations;

namespace Logement.Models
{
    public class SubscriptionPayment
    {
        public long Id { get; set; }
        public long CityId { get; set; }
        public virtual City City { get; set; }
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public DateTimeOffset NextPaymentDate { get; set; }
        public long LandLordId { get; set; }
        public bool IsPaid { get; set; }
    }
}
