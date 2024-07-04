using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    public class RentTerm
    {
        public int Id { get; set; }
        public int TenancyId { get; set; }
        public virtual Tenancy? Tenancy { get; set; }
        public RentStatusEnum RentStatus { get; set; }

        [Precision(14, 2)]
        public decimal Price { get; set; }
        public bool IsFirstPaymemt { get; set; }
        public bool IsRentPayForThisMonth { get; set; }
        public decimal DepositPrice {get;set;}
    }
}
