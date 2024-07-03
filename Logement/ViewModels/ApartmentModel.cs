using Logement.Data.Enum;
using Logement.Models;
using Microsoft.EntityFrameworkCore;

namespace Logement.ViewModels
{
    public class ApartmentModel
    {

        public long Id { get; set; }

        public string ApartmentName { get; set; } = string.Empty;
        public int CityId { get; set; }

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
    }
}
