﻿using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class CityViewModel
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name= "Neighborhood")]
        public string LocatedAt { get; set; }

        [Display(Name = "Numbers of apartment")]
        public long NumbersOfApartment { get; set; }

        [Required]
        [Display(Name = "Town")]
        public string Town { get; set; }
        public string Floor { get; set; }

        public List<IFormFile> Image { get; set; } = new List<IFormFile>();
        public byte[]? Data { get; set; }
        public string? ContentType { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
