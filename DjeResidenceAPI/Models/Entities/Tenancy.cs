using DjeResidenceAPI.Data.Enum;

namespace DjeResidenceAPI.Models.Entities
{
    public class Tenancy
    {
        public int Id { get; set; }

        public int ApartmentId { get; set; }
        public virtual Apartment Apartment { get; set; }

        public long AdderId { get; set; }
        public virtual ApplicationUser Adder { get; set; }

        public TenancyStatusEnum Status { get; set; }


        public DateTimeOffset LeaseStartDate { get; set; }
        public DateTimeOffset LeaseExpiryDate { get; set; }

        public DateTimeOffset? DateLeaseTerminate { get; set; } //In case there is a problem and the owner stop the contract
        public DateTimeOffset? DateLeaseDeleted { get; set; } // Delete to add a new one

        public bool IsLeaseDeleted { get; set; }
        public DateTimeOffset DateAdded { get; set; } = DateTimeOffset.UtcNow;

        public virtual ICollection<TenancyMember> Members { get; set; } = new List<TenancyMember>();
    }
}
