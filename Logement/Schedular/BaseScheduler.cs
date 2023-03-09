using Hangfire;
using Logement.Data;
using Logement.Models;
using Logement.Services;

namespace Logement.Schedular
{
    public class BaseScheduler
    {
        protected ApplicationDbContext _context;
        protected ILogger _logger;
        private readonly Services.EmailService _emailService;
        public BaseScheduler(ApplicationDbContext context, ILogger logger, Services.EmailService emailService)
        {
            this._context = context;
            _logger = logger;
            _emailService = emailService;
        }

        protected async Task SendConfirmationEmail(string tenantEmail, string body)
        {  
            await _emailService.SendEmailAsync(tenantEmail, "Rappel de paiement du loyer", body);
        }

        public static void Setup()
        {
           //RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.RunSchedularMethod(), "59 22 * * *");
            RecurringJob.AddOrUpdate<PaymentSchedular>(x => x.RunSchedularMethod(), "14 20 * * *");
        }
    }
}
