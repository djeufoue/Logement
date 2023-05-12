using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class CityViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name= "Located at")]
        public string LocatedAt { get; set; }

        [Display(Name = "Numbers of apartment")]
        public long NumbersOfApartment { get; set; }

        public string Floor { get; set; }

        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        public DateTime DateAdded { get; set; }
    }
}
