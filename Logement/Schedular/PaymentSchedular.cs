using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logement.Schedular
{
    public class PaymentSchedular : BaseScheduler
    {

        public PaymentSchedular(ApplicationDbContext context,
            ILogger<PaymentSchedular> logger,
            Services.EmailService emailService,
             UserManager<ApplicationUser> userManager,
             Services.SMSservice sMSservice)
            : base(context, logger, emailService, userManager, sMSservice)
        {
        }

        //Need to be strongly improve
        public async Task RunSchedularMethod()
        {
            try
            {
                //Find all active tenants
                var tenantStatus = await _context.TenantRentApartments
                                           .Where(t => t.IsActiveAsTenant == true)
                                           .ToListAsync();

                //For each active tenant 
                foreach (var tenant in tenantStatus)
                {
                    var LandLord = await _context.Apartments
                                       .Where(a => a.Id == tenant.ApartmentId)
                                       .FirstOrDefaultAsync();

                    //Cannot be null
                    ApplicationUser? LandLordinfos = await _context.Users.FindAsync(LandLord.LessorId);

                    var TenantInfos = await _userManager.FindByIdAsync(tenant.TenantId.ToString());

                    if (TenantInfos == null)
                        _logger.LogError($"User account does not exist!");
                    else
                    {
                        if (TenantInfos.Email == null && TenantInfos.PhoneNumber == null)
                        {
                            string subject = "Alerte";
                            string body = $"<p>Le locataire {TenantInfos.FirstName} n'a pas d'e-mail ou de numéro de <p>\n";
                            body += "<p>téléphone pour lui rappeler de payer son loyer; ";
                            body += "<p>veuillez contacter l'administration pour résoudre ce problème<p>";

                            await SendConfirmationEmail(LandLord.Lessor.Email, subject, body);
                        }
                        else
                        {
                            DateTime currentDate = DateTime.UtcNow;

                            //Selectionner les dates dont le status est impayé pour ce locataire actif
                            List<RentPaymentDatesSchedular> RentNotPaid = await (from r in _context.RentPaymentDatesSchedulars
                                                                                 where r.RentStatus == Data.Enum.RentStatusEnum.Unpaid &&
                                                                                       r.TenantId == tenant.TenantId &&
                                                                                       r.NextDateToPay <= currentDate
                                                                                 select new RentPaymentDatesSchedular
                                                                                 {
                                                                                     TenantId = r.TenantId,
                                                                                     NextDateToPay = r.NextDateToPay,
                                                                                     AmmountSupposedToPay = r.AmmountSupposedToPay,
                                                                                     CityId = r.CityId,
                                                                                     ApartmentNumber = r.ApartmentNumber
                                                                                 }).ToListAsync();


                            //This allows the tenant to be reminded two days before the scheduled date to pay his rent.
                            var twoDaysFromNow = currentDate.AddDays(2);

                            List<RentPaymentDatesSchedular> RentToPaySoon = await (from r in _context.RentPaymentDatesSchedulars
                                                                                 where r.RentStatus == Data.Enum.RentStatusEnum.Unpaid &&
                                                                                       r.TenantId == tenant.TenantId &&
                                                                                       r.NextDateToPay <= twoDaysFromNow
                                                                                 select new RentPaymentDatesSchedular
                                                                                 {
                                                                                     TenantId = r.TenantId,
                                                                                     NextDateToPay = r.NextDateToPay,
                                                                                     AmmountSupposedToPay = r.AmmountSupposedToPay,
                                                                                     ApartmentNumber = r.ApartmentNumber,
                                                                                     CityId = r.CityId
                                                                                 }).ToListAsync();


                            //Rents that need to be pay for this tenant
                            List<RentPaymentDatesSchedular> RentsThatNeedToBePay = new List<RentPaymentDatesSchedular>();

                            if (RentNotPaid != null)
                            {
                                foreach (var rent in RentNotPaid)
                                {
                                    //Notification la plus rescente qui a été envoy pour rappeler ce locataire de payer son loyer:
                                    var lastEmailSentDate = await _context.NotificationSentForRentPayments
                                                                .Where(d => d.TenantId == tenant.TenantId && d.ScheduledDateForRentPayment == rent.NextDateToPay)
                                                                .OrderByDescending(d => d.NotificationSentDate)
                                                                .Select(d => d.NotificationSentDate)
                                                                .FirstOrDefaultAsync();
                                 

                                    if (lastEmailSentDate != DateTime.MinValue)
                                    {
                                        var daysPassed = (currentDate - lastEmailSentDate).TotalDays;

                                        //Check if it's been more than 15 days since this tenant had to pay
                                        //This is for tenants with more than 15 days or more than a month's unpaid rent
                                        if (daysPassed > 15)
                                            RentsThatNeedToBePay.Add(rent);
                                    }
                                }

                                if (RentsThatNeedToBePay.Count != 0)
                                {
                                    await ContructMessage(RentsThatNeedToBePay, TenantInfos);
                                }
                                //Locataire qui doit bientôt payer le loyer
                                //Cela ve dire que la fin du moi est proche dont il faut deja predir 
                                //la date pour le prochain mois
                                if (RentToPaySoon.Count != 0)
                                {
                                    foreach(var rent in RentToPaySoon)
                                    {
                                        RentPaymentDatesSchedular rentPaymentDatesSchedular = new RentPaymentDatesSchedular
                                        {
                                            TenantId = TenantInfos.Id,
                                            ApartmentNumber = rent.ApartmentNumber,
                                            CityId = rent.CityId,
                                            AmountAlreadyPaid = 0,
                                            RemainingAmount = rent.AmmountSupposedToPay,
                                            RentStatus = RentStatusEnum.Unpaid,
                                            AmmountSupposedToPay = rent.AmmountSupposedToPay,
                                            NextDateToPay = DateTime.UtcNow.AddMonths(1)
                                        };
                                        _context.RentPaymentDatesSchedulars.Add(rentPaymentDatesSchedular);
                                        await _context.SaveChangesAsync();
                                    }
                                    await ContructMessage(RentToPaySoon, TenantInfos);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task ContructMessage(List<RentPaymentDatesSchedular> rentsThatNeedToPay, ApplicationUser tenantInfos)
        {

            try
            {
                //Le nonbre de mois impayé
                int count = rentsThatNeedToPay.Count();

                string body;
                if (count > 1)
                    body = "<h2>Merci de bien vouloir Payer votre loyer pour les mois suivant</h2>\n<br>";
                else
                    body = "<h2>Merci de bien vouloir Payer votre loyer de ce mois</h2>\n<br>";

                body += "<table style=\"width:100%\">";
                body += "<tr>";
                body += "<th>Numéro d'appartement</th>";
                body += "<th>Date de paiement du loyer</th>";
                body += "<th>Montant à payer</th>";
                body += "</tr>";
                foreach (var dt in rentsThatNeedToPay)
                {
                    body += "<tr>";
                    body += $"<td>{dt.ApartmentNumber}</td>";
                    body += $"<td>{dt.NextDateToPay}</td>";
                    body += $"<td>{dt.AmmountSupposedToPay}</td>";
                    body += "<tr>";

                    NotificationSentForRentPayment notificationSentForRentPayments = new NotificationSentForRentPayment
                    {
                        CityId = dt.CityId,
                        ApartmentNumber = dt.ApartmentNumber,
                        TenantId = dt.TenantId,
                        AmmountSupposedToPay = dt.AmmountSupposedToPay,
                        ScheduledDateForRentPayment = dt.NextDateToPay.DateTime,
                        NotificationSentDate = DateTime.UtcNow
                    };
                    _context.Add(notificationSentForRentPayments);
                    await _context.SaveChangesAsync();
                }
                body += "</table>";
                body += "<br>";
                body += "<br>";
                body += "<span style=\"font-weight:bold\">NB</span>: Les <span style=\"color:blue;font-weight:bold;font-style:italic\">Date de paiement du loyer</span> représentent les dates auxquelles vous deviez payer votre loyer</p>";

                string subject = "Rappel de paiement de loyer";

                //if (tenantInfos.PhoneNumber != null && tenantInfos.Email == null)
                //sendSMStoTenant(tenantInfos.PhoneNumber, body); //Pay for Twiolio Sms Service

                if (tenantInfos.Email != null && tenantInfos.PhoneNumber == null)
                    await SendConfirmationEmail(tenantInfos.Email, subject, body);

                else if (tenantInfos.Email != null && tenantInfos.PhoneNumber != null)
                {
                    //sendSMStoTenant(tenantInfos.PhoneNumber, body);
                    await SendConfirmationEmail(tenantInfos.Email, subject, body);
                }

                //Send an email to the landlord of this appartment                        
                //To do: Modifier le tritre de cette notification
                subject = $"Loyer non payé par le locataire {tenantInfos.FirstName} {tenantInfos.LastName}";
                if (tenantInfos != null)
                    await SendConfirmationEmail(tenantInfos.Email, subject, body);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task CheckSubscription()
        {
            try
            {
                DateTimeOffset currentDate = DateTimeOffset.UtcNow;

                var overdueSubscriptions = _context.SubscriptionPayments
                    .Include(s => s.City)
                    .Where(s => s.NextPaymentDate < currentDate)
                    .ToList();

                foreach (var subscription in overdueSubscriptions)
                {
                    var notificationSent = await _context.NotificationSentForSubscriptions
                        .Include(b => b.Landlord)
                        .Where(b => b.LandlordId == subscription.LandLordId && b.AmmountSupposedToPay == subscription.Amount
                            && b.NotificationSentDate >= DateTimeOffset.UtcNow.AddDays(-15))
                        .FirstOrDefaultAsync();

                    //Check if notification has already been sent in the past 15 days
                    if (notificationSent == null)
                        continue;

                    string subject = "Urgent: Payment Required for Subscription";
                    string htmlMessage = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                /* Add your custom styles here */
                            </style>
                        </head>
                        <body>
                            <h2>Urgent: Payment Required for Subscription</h2>
                            <p>Dear {notificationSent.Landlord.FirstName} {notificationSent.Landlord.LastName},</p>
                            <p>We hope this message finds you well. This is an urgent reminder regarding the outstanding payment for your subscription. Please review the details of your subscription below:</p>
                            <h3>Subscription Details:</h3>
                            <ul>
                                <li>City: {subscription.City.Name}</li>
                                <li>Amount: 5000 FCFA</li>
                                <li>Payment Due Date: {subscription.NextPaymentDate}</li>
                            </ul>
                            <p>It appears that we have not received your payment for the current subscription period. We kindly request that you settle the payment as soon as possible to ensure uninterrupted access to our services. Your prompt attention to this matter is greatly appreciated.</p>          
                            <p>Best regards,<br>[DJE RESIDENCE]</p>
                        </body>
                        </html>";

                    await SendConfirmationEmail(notificationSent.Landlord.Email, subject, htmlMessage);

                    NotificationSentForSubscription notificationSentForSubscription = new NotificationSentForSubscription
                    {
                        CityId = subscription.CityId,
                        LandlordId = subscription.LandLordId,
                        AmmountSupposedToPay = subscription.Amount,
                        NotificationSentDate = DateTime.UtcNow,
                    };

                    await _context.NotificationSentForSubscriptions.AddAsync(notificationSentForSubscription);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"An email was sent to the landlord {notificationSent.Landlord.FirstName} {notificationSent.Landlord.LastName}");
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
