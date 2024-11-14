using Logement.Data;
using Logement.DTO;
using Logement.Models;
using Logement.Schedular;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Net;

namespace Logement.Controllers
{
    [Authorize]
    public class TenancyController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<T> _logger;
        private BaseScheduler baseScheduler;
        long contractId;

        public TenancyController(ApplicationDbContext context, IConfiguration configuration,
            ILogger<T> logger, Services.EmailService emailService, UserManager<ApplicationUser> userManager,
                Services.SMSservice smsService)
            : base(context, configuration)
        {
            _userManager = userManager;
            _logger = logger;
            baseScheduler = new BaseScheduler(context, logger, emailService, userManager, smsService);
        }

        [HttpPost]
        public async Task<IActionResult> AddTenancy(TenancyDTO model)
        {
            try
            {
                string message = "";

                var currentUser = await dbc.Users
                    .Where(u => u.Id == UserId)
                    .FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    message = "The current user is not logged in or does not exist.";
                    TempData["message"] = new { Message = message, Type = "error" };
                    return StatusCode((int)HttpStatusCode.NotFound, "The current user is not logged in or does not exist");
                }

                var apartment = await dbc.Apartments
                    .Where(a => a.Id == model.ApartmentId && a.CityId == model.PropertyId)
                    .FirstOrDefaultAsync();

                if (apartment == null)
                {
                    message = "This apartment does not exist or was deleted";
                    TempData["message"] = new { Message = message, Type = "error" };
                    return StatusCode((int)HttpStatusCode.NotFound, "This apartment does not exist or was deleted");
                }

                var allUnitTenancies = await dbc.Tenancies
                    .Where(x => x.ApartmentId == model.ApartmentId)
                    .ToListAsync();

                foreach (var tncy in allUnitTenancies)
                {
                    if (tncy.LeaseExpiryDate > model.LeaseStartDate)
                    {
                        message = $"Cet appartement a déjà un contrat de bail expirant le {tncy.LeaseExpiryDate:dd/MM/yyyy}. Veuillez sélectionner une date de début après celle-ci.";
                        TempData["message"] = new { Message = message, Type = "error" };
                        return StatusCode((int)HttpStatusCode.BadRequest, message);
                    }
                }

                Tenancy tenancy = new Tenancy()
                {
                    LeaseStartDate = model.LeaseStartDate,
                    LeaseExpiryDate = model.LeaseExpiryDate,
                    DateAdded = DateTimeOffset.Now,
                    ApartmentId = apartment.Id,
                    AdderId = currentUser.Id,
                    Status = Data.Enum.TenancyStatusEnum.Pending,
                };

                dbc.Tenancies.Add(tenancy);
                await dbc.SaveChangesAsync();

                message = "Tenancy added successfully!";
                TempData["message"] = new { Message = message, Type = "success" };
                return Ok(model);
            }
            catch (Exception ex)
            {
                TempData["message"] = new { Message = "An error occurred while processing your request.", Type = "error" };
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

    }
}
