using Logement.Data;
using Logement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using NPOI.SS.Formula.Functions;
using System.Linq;

namespace Logement.Schedular
{
    public class PaymentSchedular : BaseScheduler
    {

        public PaymentSchedular(ApplicationDbContext context, ILogger<PaymentSchedular> logger, Services.EmailService emailService)
            : base(context, logger, emailService)
        {
        }


        public async Task RunSchedularMethod()
        {
            DateTime date = DateTime.Now;

            //Find all active tenants
            var tenantStatus = _context.TenantRentApartments
                                       .Where(t => t.IsActiveAsTenant == true)
                                       .Select(t => t.TenantEmail)
                                       .ToList();

            foreach (var tenantEmail in tenantStatus)
            {
                //Find all tenants who haven't paid their rent yet
                var tenantRentPaymentStatus = await _context.RentPaymentDatesSchedulars
                                                      .Where(t => t.TenantEmail == tenantEmail
                                                       && !t.IsRentPaidForThisDate)
                                                      .FirstOrDefaultAsync();

                if(tenantRentPaymentStatus != null)
                {

                    TimeSpan hours = (tenantRentPaymentStatus.NextDateToPay - DateTimeOffset.UtcNow);

                    //True if we are close to the date of payment of the rent for this month
                    if ((hours.Days) <= 1)
                    {
                        //Check if this date exists in this table
                        var PaymentDate = await _context.NotificationSentForRentPayments
                                                        .Where(d => d.ScheduledDateForRentPayment == tenantRentPaymentStatus.NextDateToPay)
                                                        .FirstOrDefaultAsync();

                        //True if a notification was already sent for this date 
                        if (PaymentDate != null)
                        {
                            //Number of days that have already passed since this notification was sent
                            TimeSpan nbrOfDaysPassed = (PaymentDate.NotificationSentDate - DateTime.UtcNow);

                            //
                            if (nbrOfDaysPassed.Days >= 30)
                            {
                                //Send a Notification
                                string body = "<h2>Merci de bien vouloir Payer votre loyer de ce mois</h2>\n";
                                body += $"<p>Montant à payer: {PaymentDate.AmmountSupposedToPay}</p>\n";
                                body += $"Passe une bonne journée!</p>\n";
                                await SendConfirmationEmail(tenantEmail, body);

                                NotificationSentForRentPayments notificationSentForRentPayments = new NotificationSentForRentPayments
                                {
                                    TenantEmail = tenantEmail,
                                    AmmountSupposedToPay = PaymentDate.AmmountSupposedToPay,

                                };
                            }
                        }
                        else
                        {
                            //Send a Notification 
                            string body = "<h2>Merci de bien vouloir Payer votre loyer de ce mois</h2>\n";
                            body += $"<p>Montant à payer: {tenantRentPaymentStatus.AmmountSupposedToPay}</p>\n";
                            body += $"Passe une bonne journée!</p>\n";
                            await SendConfirmationEmail(tenantEmail, body);
                        }
                    }
                }
            }

        }
    }
}
