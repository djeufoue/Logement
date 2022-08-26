using Logement.Models;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class AppartmentViewModel
    {
        //Some requierments are going to be added on each property
        public int Id { get; set; }

        public int? TenantId { get; set; }
        public virtual Tenant? Tenant { get; set; } // Nom du locataire actuel

        [Required]
        public string? PhotoUrl { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public int NumberOfRooms { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public string? LocatedAt { get; set; }

        
        public int NumberOfbathRooms { get; set; }

        [Required]
        public int Area { get; set; } // Superficie


        public int FloorNumber { get; set; }

        [Required]
        public string? ApartmentType { get; set; }

        
        public string? Contract { get; set; } // Rental / for Sale
        public int NumberOfParkingSpaces { get; set; }


        public int DepositePrice { get; set; } // La caution

        [Required]
        public string? ApartmentStatus { get; set; } //Busy or free
    }
}
