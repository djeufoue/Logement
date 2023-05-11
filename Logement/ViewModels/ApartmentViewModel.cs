using Logement.Data.Enum;
using Logement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Twilio.Types;

namespace Logement.ViewModels
{
    public class ApartmentViewModel: IValidatableObject
    {
        //Some requierments are going to be added on each property
        public long Id { get; set; }
        public long ApartmentNunber { get; set; }
        public long LessorId { get; set; }
        public string? CityName { get; set;}

        [Required]
        public string? Description { get; set; }

        public long? CityId { get; set; } 

        [Display(Name = "Located At")]
        public string? LocatedAt { get; set; }

        public string? OccupiedBy { get; set; }

        [Required]
        [Display(Name = "Rooms")]
        public int NumberOfRooms { get; set; }

        [Required]
        [Display(Name = "Bath Rooms")]
        public int NumberOfbathRooms { get; set; }

        [Required]
        [Display(Name = "Room Area")]
        public int RoomArea { get; set; } // Superficie

        [Display(Name ="Floor")]
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

        public ApartmentStatusEnum? Status { get; set; }

        public ApartmentTypeEnum Type { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        [Display(Name = "Apartment Image")]
        public string? ImageURL { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {           
            if(ApartmentNunber == 0)
                yield return new ValidationResult("Veuillez choisir un numéro d'appartement autre que zéro(0)!", new[] { "ApartmentNunber" });
        }
    }
}
