using DjeResidenceAPI.Data.Enum;
using System;

namespace DjeResidenceAPI.Models.Entities
{
    public class TenancyMember
    {
        public int Id { get; set; }

        public int TenancyId { get; set; }
        public virtual Tenancy Tenancy { get; set; }

        public long TenantId { get; set; }
        public virtual ApplicationUser Tenant { get; set; }

        public long AdderId { get; set; }
        public virtual ApplicationUser Adder { get; set; }

        public DateTimeOffset DateAdded { get; set; }
        public DateTimeOffset? DateDelete { get; set; }
        public DateTimeOffset? DateModified { get; set; }

        public TenancyMemberRoleEnum Role { get; set; }
        public bool SendEmail { get; set; }
    }
}
