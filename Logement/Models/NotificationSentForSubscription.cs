using System.ComponentModel;

namespace Logement.Models
{
    public class NotificationSentForSubscription
    {
        public long Id { get; set; }

        public long CityId { get; set; }
        public virtual City City { get; set; }
        public long LandlordId { get; set; }
        public virtual ApplicationUser Landlord { get; set; }
        public decimal AmmountSupposedToPay { get; set; }

       /* [Description("Date that was scheduled for the payment of the rent")]
        public DateTime ScheduledDateForRentPayment { get; set; }*/

        [Description("Date the notification was sent")]
        public DateTime NotificationSentDate { get; set; }
    }
}
