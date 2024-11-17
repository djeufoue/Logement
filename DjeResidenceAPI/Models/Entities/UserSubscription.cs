namespace DjeResidenceAPI.Models.Entities
{
    public class UserSubscription
    {
        public int Id { get; set; }
        public int LanlordId { get; set; }
        public virtual ApplicationUser Lanlord { get; set; } = null!;
        public int SubscriptionPlanId { get; set; } 
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        public DateTime StartDate { get; set; } // Subscription start date
        public DateTime EndDate { get; set; } // Subscription end date 
    }
}
