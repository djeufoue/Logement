using DjeResidenceAPI.Models.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.Xml;

namespace DjeResidenceAPI.Data.Entities
{
    public class TenantRentPayment
    {
        public int Id { get; set; }
        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }
        public long LandlordId { get; set; }
        public virtual ApplicationUser Landlord { get; set; }
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "XAF";
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
    }
}
