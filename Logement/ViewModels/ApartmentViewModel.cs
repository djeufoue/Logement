using Logement.Data.Enum;
using Logement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class ApartmentViewModel
    {
        //Some requierments are going to be added on each property
        public long Id { get; set; }
        public long LessorId { get; set; }

        [Required]
        public string? Description { get; set; }


        [Required]
        public string LocatedAt { get; set; }

        [Required]
        public int NumberOfRooms { get; set; }

        [Required]
        public int NumberOfbathRooms { get; set; }

        [Required]
        public int RoomArea { get; set; } // Superficie
        public int? FloorNumber { get; set; }

        [Required]
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
        public PaymentMethodEnum PaymentMethod { get; set; }


        /// <summary>
        /// Just the format of an unfulfilled contract 
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public long TemplateContractId { get; set; } 
        public int? NumberOfParkingSpaces { get; set; }
        /// <summary>
        /// Database needs to be migrated to add these colunm
        /// </summary>
        public ApartmentStatusEnum Status { get; set; }

        public ApartmentTypeEnum Type { get; set; }
    
    }
}
