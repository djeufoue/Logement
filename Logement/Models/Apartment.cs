using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Logement.Models
{
    public class Apartment
    {
        public int Id { get; set; }

        /// <summary>
        /// Many apartments are associated with one TenantRentStatus(One-to-Many relationship)
        /// Which mean that a tenant can have many apartment, but an apartment can have just one tenant or can have no tenant
        /// </summary>
        public int? TenantRentApartmentId { get; set; }
        public virtual TenantRentApartment? TenantRentApartment { get; set; } // Nom du locataire actuel

        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; }

        public string LocatedAt { get; set; }

        public int NumberOfRooms { get; set; }
        public int NumberOfbathRooms { get; set; }

        public int RoomArea { get; set; } // Superficie

        public int? FloorNumber { get; set; }

        // Original price of the apartment
        [Precision(14, 2)]
        public decimal Price { get; set; }

        [Precision(14, 2)]
        public decimal DepositePrice { get; set; } // La caution

        // Every apartment has his own payment method
        public PaymentMethodEnum paymentMethod { get; set; }

        public int TemplateContractId { get; set; }
        public FileModel TemplateContract { get; set; } // Rental / for Sale

        public int? NumberOfParkingSpaces { get; set; }
     
        // Database needs to be migrated to add these colunm 
        public ApartmentStatusEnum Status { get; set; }
        public ApartmentTypeEnum Type { get; set; }
    }
}
