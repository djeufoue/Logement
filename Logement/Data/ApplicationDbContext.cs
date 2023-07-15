using Logement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Logement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
        }
      
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<TenantRentApartment> TenantRentApartments { get; set; }
        public DbSet<FileModel> FileModel { get; set; }       
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
  
        public DbSet<RentPaymentDatesSchedular> RentPaymentDatesSchedulars { get; set; }
        public DbSet<NotificationSentForRentPayment> NotificationSentForRentPayments { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Fichier> Fichiers { get; set; }
        public DbSet<CityMember> CityMembers { get; set; }
        public DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
        public DbSet<NotificationSentForSubscription> NotificationSentForSubscriptions { get; set; }
    }
}
