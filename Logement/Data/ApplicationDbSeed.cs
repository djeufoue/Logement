using Logement.Controllers;
using Logement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logement.Data
{
    public static class ApplicationDbSeed
    {

        public static void SeedDatabase(IServiceProvider services)
        {
            ApplicationDbContext? dbc = services.GetService<ApplicationDbContext>();
            UserManager<ApplicationUser>? umg = services.GetService<UserManager<ApplicationUser>>();

            if (dbc != null && umg != null)
            {
                dbc.Database.Migrate();

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
                    EmailConfirmed = true,
                    JobTitle = "Software engineering",
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
