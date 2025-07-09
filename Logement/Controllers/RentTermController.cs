using Logement.Data;
using Logement.Models;
using Logement.Schedular;
using Microsoft.AspNetCore.Identity;
using NPOI.SS.Formula.Functions;

namespace Logement.Controllers
{
    public class RentTermController: BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<T> _logger;
        private BaseScheduler baseScheduler;
        long contractId;

        public RentTermController(ApplicationDbContext context, IConfiguration configuration,
            ILogger<T> logger, Services.EmailService emailService, UserManager<ApplicationUser> userManager,
                Services.SMSservice smsService)
            : base(context, configuration)
        {
            _userManager = userManager;
            _logger = logger;
            baseScheduler = new BaseScheduler(context, logger, emailService, userManager, smsService);
        }
    }
}
