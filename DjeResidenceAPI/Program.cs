using DjeResidenceAPI.Data;
using DjeResidenceAPI.Models.Entities;
using DjeResidenceAPI.Schedular;
using DjeResidenceAPI.Services;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;


namespace Logement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILogger logger = LoggerService.CreateLogger("Program");
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                LoggerService.ConfigureLogging(builder.Host);

                ConfigureDatabase(builder.Services, builder.Configuration);
                ConfigureIdentity(builder.Services);

                builder.Services.AddControllersWithViews();
                //builder.Services.AddScoped<Schedular.PaymentSchedular>();

                var app = builder.Build();

                ConfigureApp(app);
                app.Run();
            }
            catch (Exception exception)
            {
                logger.LogCritical(exception, "Server Shutdown");
                throw;
            }
            finally
            {
                LoggerService.Shutdown();
            }
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.EnableSensitiveDataLogging();
                options.UseSqlServer(connectionString);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddDistributedMemoryCache();
        }

        private static void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<EmailService>();
            services.AddSingleton<SMSservice>();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequiredLength = 6;

                // User settings.
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // SignIn settings.
                options.SignIn.RequireConfirmedAccount = false;
            });
        }
        private static void ConfigureApp(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHangfireDashboard();
            app.UseSession();

            using (var serviceScope = app.Services.CreateScope())
            {
                ApplicationDbSeed.SeedDatabase(serviceScope.ServiceProvider);
            }

            BaseScheduler.Setup();
        }
    }
}
