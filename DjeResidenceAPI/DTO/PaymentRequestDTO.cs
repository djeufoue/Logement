namespace DjeResidenceAPI.DTO
{
    public class PaymentRequestDTO
    {
        public long TenantId { get; set; }
        public long LandlordId { get; set; }
        public string TenantPhoneNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
