using Hangfire;
using Logement.Data;
using Logement.Models;
using Logement.Services;
using Microsoft.AspNetCore.Identity;

namespace Logement.Schedular
{
    public class BaseScheduler
    {
        protected ApplicationDbContext _context;
        protected ILogger _logger;
        protected readonly Services.EmailService _emailService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly Services.SMSservice _smsService;

        public BaseScheduler(ApplicationDbContext context,
            ILogger logger, Services.EmailService emailService,
            UserManager<ApplicationUser> userManager,
            Services.SMSservice smsService)
        {
            this._context = context;
            _logger = logger;
            _emailService = emailService;
            _userManager = userManager;
            _smsService = smsService;
        }

        protected async Task SendConfirmationEmail(string tenantEmail, string body)
        {  
            await _emailService.SendEmailAsync(tenantEmail, "Rappel de paiement du loyer", body);
        }

        protected async Task sendSMStoTenant(string tenantPhoneNumber, string message)
        {
            await _smsService.SendSMS(tenantPhoneNumber, message);
        }

        public static void Setup()
        {
           //RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.RunSchedularMethod(), "59 22 * * *");
           RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.RunSchedularMethod(), "19 1 * * *");
        }
    }
}
