using Logement.Data.Enum;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Logement.ViewModels
{
    public class ApartmentDescriptions
    {
        public long Id { get; set; }    
        public long LandlordId { get; set; }
        public string? LandlordPhoneNumber { get; set; }
        public string? LandlordEmail { get; set; }
        public int Price { get; set; }
        public int NumberOfRooms { get; set; }
        public int NumberOfbathRooms { get; set; }
        public int RoomArea { get; set; }
        public int FloorNumber { get; set; }
        public ApartmentTypeEnum ApartmentType { get; set; }
        public string Description { get; set; }
        public string LocatedAt { get; set; }
    }
}
