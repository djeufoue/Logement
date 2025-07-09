using DjeResidenceAPI.Data;
using DjeResidenceAPI.Data.Entities;
using DjeResidenceAPI.DTO;
using DjeResidenceAPI.Helpers;
using DjeResidenceAPI.Models.Entities;
using DjeResidenceAPI.Schedular;
using DjeResidenceAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;

namespace DjeResidenceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<T> _logger;
        private readonly OrangeMoneyService _orangeMoneyService;

        long contractId;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration,
            ILogger<T> logger, UserManager<ApplicationUser> userManager, OrangeMoneyService orangeMoneyService)
            : base(context, configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _orangeMoneyService = orangeMoneyService;
        }

        [HttpPost("tenant-to-landlord")]
        public async Task<IActionResult> TenantToLandlordPayment([FromBody] PaymentRequestDTO request)
        {
            try
            {
                var landlord = await dbc.Users
                    .Where(l => l.Id == request.LandlordId)
                    .FirstOrDefaultAsync();

                if (landlord == null)
                    return NotFound(new { Error = "Landlord not found." });

                string landlordPhone;

                if (!string.IsNullOrEmpty(landlord.PhoneNumber))
                {
                    landlordPhone = landlord.CountryCode + landlord.PhoneNumber;
                }
                else
                {
                    return BadRequest("Your lanloard does not have a phone number.");
                }

                // Initiate payment
                var transactionId = Guid.NewGuid().ToString();

                var paymentResponse = await _orangeMoneyService.TransferMoney(
                    request.TenantPhoneNumber,
                    landlordPhone,
                    request.Amount,
                    transactionId
                );

                // Save payment record with status "PENDING"
                var payment = new TenantRentPayment
                {
                    TenantId = request.TenantId,
                    LandlordId = request.LandlordId,
                    Amount = request.Amount,
                    Currency = "XAF",
                    TransactionId = transactionId,
                    Status = "PENDING" // Initial status
                };

                dbc.TenantRentPayments.Add(payment);
                await dbc.SaveChangesAsync();

                // Fetch transaction status
                var statusResponse = await _orangeMoneyService.GetTransactionStatus(transactionId);

                // Update payment status
                payment.Status = PaymentHelpers.ParseTransactionStatus(statusResponse); 
                dbc.TenantRentPayments.Update(payment);
                await dbc.SaveChangesAsync();

                return Ok(new
                {
                    TransactionId = transactionId,
                    PaymentResponse = paymentResponse,
                    StatusResponse = statusResponse
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
