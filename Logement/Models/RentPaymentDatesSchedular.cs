using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    //[Index(nameof(NextDateToPay), IsUnique = true)]
    public class RentPaymentDatesSchedular
    {
        public long Id { get; set; }
        public string TenantEmail { get; set; }

        public decimal AmmountSupposedToPay { get; set; }
        public bool IsRentPaidForThisDate { get; set; }
        public DateTimeOffset NextDateToPay { get; set; }
    }
}
