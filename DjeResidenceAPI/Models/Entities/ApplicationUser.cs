using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DjeResidenceAPI.Models.Entities
{
    public class ApplicationUser : IdentityUser<long>
    {
        [MaxLength(256)]
        public override string PasswordHash { get; set; }

        [MaxLength(256)]
        public override string SecurityStamp { get; set; }

        [MaxLength(256)]
        public override string ConcurrencyStamp { get; set; }

        [MaxLength(256)]
        public override string PhoneNumber { get; set; }
        public string CountryCode { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
    }


    public class ApplicationRole : IdentityRole<long>
    {
        [MaxLength(256)]
        public override string ConcurrencyStamp { get; set; }
    }
}
