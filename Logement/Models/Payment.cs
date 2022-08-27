
using Microsoft.EntityFrameworkCore;

namespace Logement.Models
{
    public class Payment
    {
        public long Id { get; set; }

        public long TenantRentApartmentId { get; set; }
        public virtual TenantRentApartment TenantRentApartment  { get; set;}

        
        public long LessorId { get; set; }

        /// <summary>
        /// The only person who can make changes on the payments
        /// </summary>
        public virtual ApplicationUser Lessor { get; set; }

        [Precision(14, 2)]
        public decimal Amount { get; set; }
         
        public DateTime DatePaid { get; set; }

    }
}
