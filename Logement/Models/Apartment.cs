using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Logement.Models
{
    public class Apartment
    {
        public long Id { get; set; }

        /// <summary>
        /// The lessor is the only person who can make 
        /// changes (add, delete, update information) on the apartments
        /// </summary>
        public long LessorId { get; set; }
        public virtual ApplicationUser Lessor { get; set; }

        [Required]
        [MaxLength(2000)]
        public string? Description { get; set; }

        public string LocatedAt { get; set; }

        public int NumberOfRooms { get; set; }
        public int NumberOfbathRooms { get; set; }

        public int RoomArea { get; set; } // Superficie

        public int? FloorNumber { get; set; }

        /// <summary>
        /// Original price of the apartment
        /// </summary>
        [Precision(14, 2)]
        public decimal Price { get; set; }

        /// <summary>
        /// La caution
        /// </summary>
        [Precision(14, 2)]
        public decimal DepositePrice { get; set; } 

        /// <summary>
        /// Every apartment has his own payment method
        /// </summary>
        public PaymentMethodEnum paymentMethod { get; set; }

        /// <summary>
        /// Just the format of an unfulfilled contract 
        /// </summary>
        public long TemplateContractId { get; set; }
        public virtual FileModel TemplateContract { get; set; }

        public int? NumberOfParkingSpaces { get; set; }

        /// <summary>
        /// Database needs to be migrated to add these colunm
        /// </summary>
        public ApartmentStatusEnum Status { get; set; }

        public ApartmentTypeEnum Type { get; set; }

    }
}
