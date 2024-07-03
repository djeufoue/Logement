using Logement.Data.Enum;
using Logement.Models;

namespace Logement.DTO
{
    public class TenancyDTO
    {
        public DateTimeOffset LeaseStartDate { get; set; }
        public DateTimeOffset LeaseExpiryDate { get; set; }
        public int ApartmentId { get; set; }
    }
}
