using Logement.Data.Enum;

namespace Logement.Models
{
    public class CityMember
    {
        public long Id { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }

        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public long? ApartmentId { get; set;}
        public virtual Apartment? Apartment { get; set; }

        public CityMemberRoleEnum Role { get; set; }
    }
}
