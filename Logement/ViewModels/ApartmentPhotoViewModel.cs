using Logement.Models;
using Microsoft.Build.Framework;

namespace Logement.ViewModels
{
    public class ApartmentPhotoViewModel
    {
        public long Id { get; set; }

        public long ApartmentId { get; set; }

        /// <summary>
        /// Indicate which part of the apartment the photo is for (living room bed room or toilet, kitchen...)
        /// </summary>
        [Required]
        public string Part { get; set; }

        [Required]
        public IFormFile ImageURL { get; set; }
    }
}
