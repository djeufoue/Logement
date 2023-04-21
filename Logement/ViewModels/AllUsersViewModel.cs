using Logement.Data.Enum;
using Microsoft.AspNetCore.Server.HttpSys;

namespace Logement.ViewModels
{
    public class AllUsersViewModel
    {
        public long Id { get; set; }
        public long? CityId { get; set; }   
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public MaritalStatusEnum? MaritalStatus { get; set; }
        public string JobTitle { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
