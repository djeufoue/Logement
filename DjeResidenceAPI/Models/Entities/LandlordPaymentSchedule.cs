using System.ComponentModel.DataAnnotations.Schema;

namespace DjeResidenceAPI.Models.Entities
{
    public class LandlordPaymentSchedule
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } // Amount to pay based on his chosen plan
        public DateTimeOffset PaymentDate { get; set; }
        public DateTimeOffset NextPaymentDate { get; set; }
        public long LandLordId { get; set; }
        public bool IsPaid { get; set; }
    }
}
