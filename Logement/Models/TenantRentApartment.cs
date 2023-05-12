using Logement.Data.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;

namespace Logement.Models
{
    [Index(nameof(ApartmentId), IsUnique = true)]
    public class TenantRentApartment
    {
        public long Id { get; set; }

        /// <summary>
        /// An apartment can belong to only one Tenant 
        /// </summary>
        public long? ApartmentId { get; set; }
        public virtual Apartment? Apartment { get; set; } // Nom du locataire actuel

        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }

        public string? TenantPhoneNumber { get; set; }

        /// <summary>
        /// Contrat de bail
        /// </summary>
        public long? BailId { get; set; }
        public virtual FileModel? Bail { get; set; }

        /// <summary>
        /// Price after nagociations
        /// </summary>
        [Precision(14, 2)]
        public decimal Price { get; set; }

        public decimal AmountPaidByTenant { get; set; }

        /// <summary>
        /// La caution qui a ete paye
        /// </summary>
        [Precision(14, 2)]
        public decimal DepositePrice { get; set; }

        public PaymentMethodEnum PaymentMethodEnum { get; set; }

        /// <summary>
        /// It is on this date that the tenant begins to pay his rent.
        /// </summary>
        public DateTime StartOfContract { get; set; }

        public DateTime? EndOfContract { get; set; }

        public bool IsActiveAsTenant { get; set; }
    }
}
