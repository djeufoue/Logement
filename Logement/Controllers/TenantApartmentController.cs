using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Net;

namespace Logement.Controllers
{
    [Authorize(Roles = "Tenant,Admin,SystemAdmin")]
    public class TenantApartmentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        protected readonly ILogger<T> _logger;

        public TenantApartmentController( ApplicationDbContext context, ILogger<T> logger)
        {
            _context = context;
            _logger = logger;
        }


        //Get the rent status payment of a specific tenant
        [HttpGet]
        public IActionResult TenantRentStatus(string tenantEmail)
        {
            TenantPaymentStatusViewModel tenantPaymentStatusViewModel = new TenantPaymentStatusViewModel();
            ApplicationUser getTenant = new ApplicationUser();

            var tenantStatus = _context.TenantPaymentStatuses
                                       .Where(t => t.TenantEmail == tenantEmail)
                                       .FirstOrDefault();
          
            //Not suppose to be null
             getTenant = _context.Users
                                 .Where(u => u.Email == tenantStatus.TenantEmail)
                                 .FirstOrDefault();

            if (tenantStatus != null)
            {
                tenantPaymentStatusViewModel = new TenantPaymentStatusViewModel()
                {
                    TenantName = getTenant.TenantFirstName,
                    TenantEmail = tenantStatus.TenantEmail,
                    AmountRemainingForRent = tenantStatus.AmountRemainingForRent,
                    NumberOfMonthsToPay = tenantStatus.NumberOfMonthsToPay,
                    RentStatus = tenantStatus.RentStatus
                };
            }
            return View(tenantPaymentStatusViewModel);
        }


        public PaymentHistoryViewModel GetPaymentFromModel(PaymentHistory payments)
        {
            PaymentHistoryViewModel allPayments = new PaymentHistoryViewModel()
            {
                Id = payments.Id,
                TenantEmail = payments.TenantEmail,
                AmountPaid = payments.AmountPaid,
                NunberOfMonthPaid = payments.NunberOfMonthPaid,
                PaidDate = payments.PaidDate
            };
            return allPayments;
        }


        public IActionResult GetAllPaymentsHistory(string userEmail)
        {
            List<PaymentHistoryViewModel> paymentHistory = new List<PaymentHistoryViewModel>();

            //Payments for a specific tenant
            var payments = _context.PaymentHistories
                                .Where(t => t.TenantEmail == userEmail)
                                .ToList();
            if (payments != null)
            {
                foreach (PaymentHistory payment in payments)
                {
                    paymentHistory.Add(GetPaymentFromModel(payment));
                }
                return View(paymentHistory);
            }
            else
            {
                return NotFound("No paymentHistory for this user");
            }            
        }

        protected ActionResult InternalServerError(Exception ex, string? message = null)
        {
            _logger.LogError(ex, message);
            if (message == null)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                message = ex.Message;
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, message);
        }
    }  
}
