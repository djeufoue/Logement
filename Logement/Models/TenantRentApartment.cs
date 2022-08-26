using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    public class TenantRentApartment
    {
        public int Id { get; set; }
        
        public int TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// can be zero(0)
        /// </summary>
        public int NumberOfMonthsToPay { get; set; }  

        /// <summary>
        /// Contrat de bail
        /// </summary>
        public int BailId { get; set; }
        public virtual FileModel Bail { get; set; } 

        public int AmountRemainingForRent { get; set; }

        /// <summary>
        /// Price after nagociations
        /// </summary>
        [Precision(14, 2)]
        public decimal Price { get; set; }

        /// <summary>
        /// La caution qui a ete paye
        /// </summary>
        [Precision(14, 2)]
        public decimal DepositePrice { get; set; }
    }
}
