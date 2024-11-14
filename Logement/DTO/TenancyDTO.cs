using Logement.Data.Enum;
using Logement.Models;
using System.ComponentModel.DataAnnotations;

namespace Logement.DTO
{
    public class TenancyDTO
    {
        [Required(ErrorMessage = "Lease Start Date is required.")]
        public DateTimeOffset LeaseStartDate { get; set; }

        [Required(ErrorMessage = "Lease Expiry Date is required.")]
        public DateTimeOffset LeaseExpiryDate { get; set; }
        public string? Status { get; set; }
        public int? TenancyId { get; set; }
        public int ApartmentId { get; set; }
        public long PropertyId { get; set; }
    }
}
