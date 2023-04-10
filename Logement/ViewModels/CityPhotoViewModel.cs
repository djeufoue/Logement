using Logement.Models;

namespace Logement.ViewModels
{
    public class CityPhotoViewModel
    {
        public long Id { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }

        public long Size { get; set; }
        public IFormFile ImageURL { get; set; }
    }
}
