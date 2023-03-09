﻿using System.ComponentModel;

namespace Logement.Models
{
    public class NotificationSentForRentPayments
    {
        public long Id { get; set; }
        public string TenantEmail { get; set; }

        public decimal AmmountSupposedToPay { get; set; }

        [Description("Date that was scheduled for the payment of the rent")]
        public DateTime ScheduledDateForRentPayment { get; set; }

        [Description("Date the notification was sent")]
        public DateTime NotificationSentDate { get; set; }
    }
}
