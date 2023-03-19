using Logement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Logement.ViewModels;

namespace Logement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
        }

       
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<ApplicationUser> TenantsInfos { get; set; }
        public DbSet<TenantRentApartment> TenantRentApartments { get; set; }
        public DbSet<ApartmentPhoto> ApartmentPhotos { get; set; }

        public DbSet<FileModel> FileModel { get; set; }
        
        public DbSet<PaymentHistory> PaymentHistories { get; set; }

        public DbSet<TenantPaymentStatu> TenantPaymentStatuses { get; set; }
        public DbSet<RentPaymentDatesSchedular> RentPaymentDatesSchedulars { get; set; }
        public DbSet<NotificationSentForRentPayment> NotificationSentForRentPayments { get; set; }
        


    }
}
