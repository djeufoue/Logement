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
        public override string? PhoneNumber { get; set; }

        public override string Email { get => base.Email; set => base.Email = value; }

        public DateTime DatOfBirth { get; set; }
    }


    public class ApplicationRole : IdentityRole<long>
    {
        [MaxLength(256)]
        public override string? ConcurrencyStamp { get; set; }
    }
}
