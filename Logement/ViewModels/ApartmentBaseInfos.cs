using Logement.DTO;

namespace Logement.ViewModels
{
    public class ApartmentBaseInfos
    {
        public ApartmentDescriptions ApartmentInfos = new ApartmentDescriptions();  
        public List<ApartmentImagesInfos> ApartmentImages = new List<ApartmentImagesInfos>();
        public List<TenancyDTO>? ApartmentTenancies = new List<TenancyDTO>();
    }
}
