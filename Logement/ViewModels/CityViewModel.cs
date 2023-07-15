using Logement.Data.Enum;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Logement.ViewModels
{
    public class CityViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [DisplayName("Apartment Number")]
        public long? ApartmentNumber { get; set; }
        public string? LandlordFullName { get; set; }

        [Required]
        [Display(Name= "Neighborhood")]
        public string LocatedAt { get; set; }

        [Display(Name = "Numbers of apartment")]
        public long NumbersOfApartment { get; set; }
        public CityMemberRoleEnum? CityMemberRole { get; set; }

        [Required]
        [Display(Name = "Town")]
        public string Town { get; set; }
        public string Floor { get; set; }

        public IFormFile? Image { get; set; }
        public byte[]? Data { get; set; }
        public string? ContentType { get; set; }
        public DateTime DateAdded { get; set; }

        public long? SubcriptionId { get; set; }
        public DateTimeOffset NextPaymentDate { get; set; }
    }
}
