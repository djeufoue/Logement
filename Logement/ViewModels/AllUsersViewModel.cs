using Logement.Data.Enum;
using Microsoft.AspNetCore.Server.HttpSys;
using System.ComponentModel.DataAnnotations;

namespace Logement.ViewModels
{
    public class AllUsersViewModel
    {
        public long Id { get; set; }
        public long? CityId { get; set; }

        [Display(Name = "Full name")]
        public string TenantFullName { get; set; }

        public long TenantId { get; set; }

        public MaritalStatusEnum? MaritalStatus { get; set; }
        public string JobTitle { get; set; }

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}
