using Logement.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class AppartmentViewModel
    {
        //Some requierments are going to be added on each property
        public int Id { get; set; }

        /// <summary>
        /// Access to current tenant information if there is one for this apartment
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int? TenantId { get; set; }

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

        [Required]
        public int NumberOfbathRooms { get; set; }

        [Required]
        public int Area { get; set; } // Superficie


        public int? FloorNumber { get; set; }

        [Required]
        public string? ApartmentType { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int TemplateContractId { get; set; }

        public string? TemplateContract { get; set; }

        public int NumberOfParkingSpaces { get; set; }

        /// <summary>
        ///  La caution juste pour afficher sur le site
        /// </summary>
        public int DepositePrice { get; set; }

        [Required]
        public string? ApartmentStatus { get; set; } //Busy or free
    }
}
