namespace Logement.Models
{
    public class Apartment
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public Tenant? Tenant { get; set; } // Nom du locataire actuel
        public string? PhotoUrl { get; set; }
        public string? Description { get; set; }
        public int NumberOfRooms { get; set; }
        public int Price { get; set; }
        public string? LocatedAt { get; set; }
        public int NumberOfbathRooms { get; set; }
        public int Area { get; set; }
        public int FloorNumber { get; set; }
        public string? ApartmentType { get; set; }
        public string? Contract { get; set; } // Rental / for Sale
        public int NumberOfParkingSpaces { get; set; }
        public int DepositePrice { get; set; } // La caution
        public string? ApartmentStatus { get; set; } //Busy or free
    }
}
