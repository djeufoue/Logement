namespace Logement.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public int ApartmentId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string? TenantName { get; set; }
        public bool Married { get; set; }
        public string? JobTitle { get; set; }
        public string? MaritalStatus { get; set; }
        public int ApartmentOccupies { get; set; }
        public virtual Apartment? Apartment { get; set; }
        public string? RentStatus { get; set; }
        public int? NumberOfMonthsToPay { get; set; }
        public DateTime EffectiveDateOfRent { get; set; }
        public int? Bail { get; set; }
        public DateTime CurrentRentPaymentDate { get; set; }
        public int? AmountRemainingForRent { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
