namespace Logement.ViewModels
{
    public class CityHomePageViewModel
    {
        public List<CityViewModel> CityViewModel { get; set; } = new List<CityViewModel>();
        public List<ApartmentInfos> Apartment { get; set; } = new List<ApartmentInfos>();
        public string? FirstImage { get; set; }
    }
}
