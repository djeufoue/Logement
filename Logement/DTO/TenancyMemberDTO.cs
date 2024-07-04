using Logement.Data.Enum;
using Logement.Models;

namespace Logement.DTO
{
    public class TenancyMemberDTO
    {
        public int TenancyId { get; set; }

        public long TenantId { get; set; }

        public string Role { get; set; } = string.Empty;
        public bool SendEmail { get; set; }
    }
}
