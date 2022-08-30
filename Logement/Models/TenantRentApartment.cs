using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    /// <summary>
    /// I will create a view model for this table which will allow the tenant to 
    /// have access to his information (without being able to modify anything on 
    /// this of course) and the lessor will also be able to see this same 
    /// information and modify it if he wants
    /// 
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

        /// <summary>
        /// There are tenants who pay by the month and others 
        /// after every two months or even by the year; 
        /// it depends on the arrangement he has with his owner
        /// </summary>
        public PaymentMethodEnum paymentMethodEnum { get; set; }

        /// <summary>
        /// It is on this date that the tenant begins to pay his rent.
        /// </summary>
        public DateTimeOffset StartOfContract { get; set; }

        /// <summary>
        /// This marks the end of the contract that the tenant had signed 
        /// with the lessor, but does not mark the end of the payment of 
        /// the rent: "it is possible that the tenant still owes money to 
        /// the lessor after the end of his contract"
        /// </summary>
        public DateTimeOffset? EndOfContract { get; set; }
    }
}
