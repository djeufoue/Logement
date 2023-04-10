using Logement.Models;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class CityViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string LocatedAt { get; set; }
        public long NumbersOfApartment { get; set; }

        public string Floor { get; set; }
        public IFormFile CityImage { get; set; }

        public string? CityPhoto { get; set; }

        public int? NumberOfParkingSpaces { get; set; }
        //public ICollection<CityPhotoViewModel> CityPhotos { get; set; }
        //public CityPhotoViewModel GetCityPhotoViewModel { get; set; } = new CityPhotoViewModel();
    }
}
