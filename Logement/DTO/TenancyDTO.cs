using Logement.Data.Enum;
using Logement.Models;

namespace Logement.DTO
{
    public class TenancyDTO
    {
        public DateTimeOffset LeaseStartDate { get; set; }
        public DateTimeOffset LeaseExpiryDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TenancyId { get; set; }
        public int ApartmentId { get; set; }
    }
}
