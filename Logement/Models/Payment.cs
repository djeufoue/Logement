
using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int tenantRentApartmentId { get; set; }
        public virtual TenantRentApartment TenantRentApartment  { get; set;}

        [Precision(14, 2)]
        public decimal Amount { get; set; }
         
        public DateTime DatePaid { get; set; }

    }
}
