using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Net;

namespace Logement.Controllers
{
    [Authorize(Roles = "SystemAdmin")]
    public class SubscriptionController : BaseController
    {

        UserManager<ApplicationUser> _userManager;
        public SubscriptionController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
            : base(context, configuration)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Check if current user is the systemAdmin 
                var isInRole = await _userManager.IsInRoleAsync(GetUser(), "SystemAdmin");

                if (isInRole)
                {
                    var cities = await dbc.Cities
                        .Include(c => c.LandLord)
                        .ToListAsync();

                    List<CityViewModel> cityViewModels = new List<CityViewModel>();
                    foreach (var city in cities)
                    {
                        var subscriptionStatus = await dbc.SubscriptionPayments
                                .Where(s => s.CityId == city.Id)
                                .FirstOrDefaultAsync();

                        cityViewModels.Add(new CityViewModel
                        {
                            Id = city.Id,
                            Name = city.Name,
                            LandlordFullName = $"{city.LandLord.FirstName} {city.LandLord.LastName}",
                            LocatedAt = city.LocatedAt,
                            NumbersOfApartment = city.NumbersOfApartment,
                            Town = city.Town,
                            Floor = city.Floor,
                            SubcriptionId = subscriptionStatus == null? 0: subscriptionStatus.Id,
                            NextPaymentDate = subscriptionStatus == null ? DateTimeOffset.MinValue : subscriptionStatus.NextPaymentDate,
                        });
                    }
                    return View(cityViewModels);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task<IActionResult> ExtendSubscriptionDate( long? subcriptionId,  long cityId)
        {
            try
            {
                var isInRole = await _userManager.IsInRoleAsync(GetUser(), "SystemAdmin");

                if (isInRole)
                {
                    var city = await dbc.Cities.FindAsync(cityId);
                    if (city == null)
                        return NotFound();

                    var citySubcription = await dbc.SubscriptionPayments
                        .Where(s => s.Id == subcriptionId)
                        .FirstOrDefaultAsync();

                    if (citySubcription == null)
                    {
                        SubscriptionPayment subscriptionPayment = new SubscriptionPayment()
                        {
                            PaymentId = "PAYID-" + Guid.NewGuid().ToString("N"),
                            CityId = cityId,
                            Amount = 5000,
                            PaymentDate = DateTime.UtcNow,
                            NextPaymentDate = DateTime.UtcNow.AddMonths(1),
                            LandLordId = city.LandLordId,
                            IsPaid = true,
                        };
                        dbc.SubscriptionPayments.Add(subscriptionPayment);
                        await dbc.SaveChangesAsync();

                        return Redirect("/Subscription/Index");
                    }
                    else
                    {
                        citySubcription.PaymentId = "PAYID-" + Guid.NewGuid().ToString("N");
                        citySubcription.PaymentDate = DateTime.UtcNow;
                        citySubcription.NextPaymentDate = DateTime.UtcNow.AddMonths(1);

                        dbc.Update(citySubcription);
                        await dbc.SaveChangesAsync();
                        return Redirect("/Subscription/Index");
                    }
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
