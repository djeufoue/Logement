using Logement.Data.Enum;
using Logement.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Logement.Models
{
    [Index(nameof(ApartmentName), IsUnique = true)]
    public class Apartment
    {
        public long Id { get; set; }

        public string ApartmentName { get; set; } = string.Empty;
        public long ApartmentAdderId { get; set; }
        public virtual ApplicationUser? ApartmentAdder { get; set; }
        public long ApartmentNumber { get; set; }
        public long LessorId { get; set; }
        public virtual ApplicationUser Lessor { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }

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

        public DateTime CreatedOn { get; set; }
        public int MinimunTenancyPeriod { get; set; }
    }
}
