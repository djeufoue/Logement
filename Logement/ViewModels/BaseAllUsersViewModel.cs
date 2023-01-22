using Logement.Data.Enum;

namespace Logement.ViewModels
{
    public class BaseAllUsersViewModel
    {
        public long ApartmentId { get; set; }

        public IEnumerable<AllUsersViewModel> Users { get; set; }
    }
}
