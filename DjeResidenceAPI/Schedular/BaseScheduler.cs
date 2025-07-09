using DjeResidenceAPI.Data;
using DjeResidenceAPI.Models.Entities;
using Hangfire;
using Microsoft.AspNetCore.Identity;

namespace DjeResidenceAPI.Schedular
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

        public async Task<bool> SendEmail(string tenantEmail, string subject, string body)
        {
            return await _emailService.SendEmailAsync(tenantEmail, subject, body);
        }

        public async Task<bool> sendSMStoTenant(string tenantPhoneNumber, string htmlBody)
        {
            return await _smsService.SendNewSMSAsync(tenantPhoneNumber, htmlBody);
        }

        public static void Setup()
        {
            //RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.RunSchedularMethod(), "59 22 * * *");
            //RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.RunSchedularMethod(), "47 16 * * *");
            //RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.CheckSubscription(), "*/10 * * * *");
        }
    }
}
