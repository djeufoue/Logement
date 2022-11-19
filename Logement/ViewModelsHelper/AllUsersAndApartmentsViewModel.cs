using Logement.ViewModels;

namespace Logement.ViewModelsHelper
{
    public class AllUsersAndApartmentsViewModel
    {
        public List<ApartmentViewModel> ApartmentViewModel { get; set; }
        public List<AllUsersViewModel> allUsersViewModel { get; set; }
    }
}
