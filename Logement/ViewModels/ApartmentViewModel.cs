using Logement.Data.Enum;
using Logement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [Display(Name = "Rooms")]
        public int NumberOfRooms { get; set; }

        [Required]
        [Display(Name = "Bath Rooms")]
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
        [Display(Name = "Deposite Price")]
        [Precision(14, 2)]
        public decimal DepositePrice { get; set; }

        /// <summary>
        /// Every apartment has his own payment method
        /// </summary>
        [Display(Name = "Payment Method")]
        public PaymentMethodEnum PaymentMethod { get; set; }


        /// <summary>
        /// Just the format of an unfulfilled contract 
        /// </summary>
        [HiddenInput(DisplayValue = true)]
        public long TemplateContractId { get; set; }

        [Display(Name = "Parkings")]
        public int? NumberOfParkingSpaces { get; set; }
        /// <summary>
        /// Database needs to be migrated to add these colunm
        /// </summary>
        public ApartmentStatusEnum Status { get; set; }

        public ApartmentTypeEnum Type { get; set; }

        [NotMapped]
        [Display(Name = "Apartment Image")]
        public IFormFile ImageFile { get; set; }

        /*[Display(Name = "Image")]
        public string? ImageURL { get; set; }*/

        [Display(Name = "Which part")]
        public string Part { get; set; }

        [NotMapped]
        [Display(Name = "Upload template Contract")]
        public IFormFile UploadTemplateContract { get; set; }


        [Display(Name = "Apartment Image")]
        public string ImageURL { get; set; }
    }
}
