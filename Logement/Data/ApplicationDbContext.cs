﻿using Logement.Models;
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
        public DbSet<Payment> Payments { get; set; }
        public DbSet<TenantRentApartment> TenantRentApartments { get; set; }
        public DbSet<ApartmentPhoto> ApartmentPhotos { get; set; }

        public DbSet<FileModel> FileModel { get; set; }

        public DbSet<Logement.ViewModels.TenantRentApartmentViewModel> TenantRentApartmentViewModel { get; set; }
    }
}
