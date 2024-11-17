using DjeResidenceAPI.Data.Enum;

namespace DjeResidenceAPI.Models.Entities
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public SubscriptionPlanEnum PlanName { get; set; }
        public decimal MonthlyCost { get; set; } // Cost per month
        public decimal? YearlyCost { get; set; } // Cost per year, null if not applicable
        public string Features { get; set; } = string.Empty; // JSON or text to store plan features
        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>(); 
    }
}
