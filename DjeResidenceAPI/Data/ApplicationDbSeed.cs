using DjeResidenceAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DjeResidenceAPI.Data
{
    public class ApplicationDbSeed
    {
        public static void SeedDatabase(IServiceProvider services)
        {
            ApplicationDbContext dbc = services.GetService<ApplicationDbContext>();
            UserManager<ApplicationUser> umg = services.GetService<UserManager<ApplicationUser>>();

            if (dbc != null && umg != null)
            {
                //dbc.Database.Migrate();

                SeedRoles(dbc);
                SeedAdmin(umg, dbc);
            }
        }

        private static void SeedRoles(ApplicationDbContext dbc)
        {
            if (!dbc.Roles.Any())
            {
                var roles = new List<ApplicationRole>()
                {
                    new ApplicationRole { Name = "SystemAdmin", NormalizedName = "SYSTEMADMIN" },
                };
                dbc.Roles.AddRange(roles);
                dbc.SaveChanges();
            }
        }

        private static void SeedAdmin(UserManager<ApplicationUser> userManager, ApplicationDbContext dbc)
        {
            if (!dbc.Users.Any())
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = "djeufoueadrien@gmail.com",
                    Email = "djeufoueadrien@gmail.com",
                    CountryCode = "237",
                    EmailConfirmed = true,
                    FirstName = "Adrien",
                    LastName = "Lontsi",
                };

                IdentityResult result = userManager.CreateAsync(user, "Password_1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "SystemAdmin").Wait();
                }
            }
        }
    }
}
