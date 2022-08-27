using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    [Index(nameof(ApartmentId), IsUnique = true)]
    public class TenantRentApartment
    {
        public long Id { get; set; }

        /// <summary>
        /// An apartment can belong to only one Tenant 
        /// </summary>
        public long ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; } // Nom du locataire actuel

        /// <summary>
        /// The tenant who occupies the apartment
        /// </summary>
        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }

        /// <summary>
        /// can be zero(0)
        /// </summary>
        public int NumberOfMonthsToPay { get; set; }  

        /// <summary>
        /// Contrat de bail
        /// </summary>
        public long BailId { get; set; }
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
