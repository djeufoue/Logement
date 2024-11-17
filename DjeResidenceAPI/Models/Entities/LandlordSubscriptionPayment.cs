namespace DjeResidenceAPI.Models.Entities
{
    public class LandlordSubscriptionPayment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }

        public long LandlordId { get; set; }
        public virtual ApplicationUser Landlord { get; set; }
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "XAF";
        public string Status { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
    }
}
