using DjeResidenceAPI.Data.Entities;
using DjeResidenceAPI.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DjeResidenceAPI.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<LandlordSubscriptionPayment> LandlordSubscriptionPayments { get; set; }
        public DbSet<LandlordPaymentSchedule> LandlordPaymentSchedules { get; set; }
        public DbSet<TenantRentPayment> TenantRentPayments { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Tenancy> Tenancies { get; set; }
        public DbSet<TenancyMember> TenancyMembers { get; set; }      
        public DbSet<Document> Documents { get; set; }
     }
}
