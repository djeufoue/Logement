namespace Logement.ViewModels
{
    public class CityHomePageViewModel
    {
        public List<CityViewModel> CityViewModel { get; set; } = new List<CityViewModel>();
        public List<ApartmentViewModel> ApartmentViewModel { get; set; } = new List<ApartmentViewModel>();
        public string? FirstImage { get; set; }
    }
}
