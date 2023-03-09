using Logement.Data.Enum;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Logement.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        [MaxLength(256)]
        public override string? PasswordHash { get; set; }

        [MaxLength(256)]
        public override string? SecurityStamp { get; set; }

        [MaxLength(256)]
        public override string? ConcurrencyStamp { get; set; }

        [MaxLength(256)]
        public override string PhoneNumber { get; set; }

        public string TenantFirstName { get; set; }

        public string TenantLastName { get; set; }

        // Need to be seed
        public MaritalStatusEnum? MaritalStatus { get; set; }

        public string JobTitle { get; set; }

    }


    public class ApplicationRole : IdentityRole<long>
    {
        [MaxLength(256)]
        public override string? ConcurrencyStamp { get; set; }
    }
}
