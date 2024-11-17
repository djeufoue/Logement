using DjeResidenceAPI.Data.Enum;
using Microsoft.EntityFrameworkCore;

namespace DjeResidenceAPI.Models.Entities
{
    public class Apartment
    {
        public long Id { get; set; }

        public string ApartmentName { get; set; }
        public long AdderId { get; set; }
        public virtual ApplicationUser Adder { get; set; }
        public long ApartmentNumber { get; set; }
        public long LandlordId { get; set; }
        public virtual ApplicationUser Landlord { get; set; }

        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }

        public int NumberOfRooms { get; set; }
        public int NumberOfbathRooms { get; set; }

        public int RoomArea { get; set; } // Superficie

        public int? FloorNumber { get; set; }

        [Precision(14, 2)]
        public decimal Price { get; set; }

        [Precision(14, 2)]
        public decimal DepositePrice { get; set; }

        public ApartmentStatusEnum? Status { get; set; }

        public ApartmentTypeEnum Type { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
