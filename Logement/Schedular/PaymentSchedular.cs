using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Messaging;

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
                                       .Include(a => a.City)
                                       .FirstOrDefaultAsync();

                    //Cannot be null
                    ApplicationUser? landLordinfos = await _context.Users.FindAsync(LandLord.LessorId);

                    if (landLordinfos != null)
                    {
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
                                DateTimeOffset currentDate = DateTimeOffset.UtcNow;

                                //(t.RentStatus == RentStatusEnum.Unpaid || t.RentStatus == RentStatusEnum.Partially_paid)
                                //Selectionner les dates dont le status est impayé pour ce locataire actif
                                List<RentPaymentDatesSchedular> RentNotPaid = await (from r in _context.RentPaymentDatesSchedulars
                                                                                     where (r.RentStatus == Data.Enum.RentStatusEnum.Unpaid || r.RentStatus == Data.Enum.RentStatusEnum.Partially_paid) &&
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
                                //Probleme to be solve
                                List<RentPaymentDatesSchedular> RentToPaySoon = await (from r in _context.RentPaymentDatesSchedulars
                                                                                where (r.RentStatus == Data.Enum.RentStatusEnum.Unpaid || r.RentStatus == Data.Enum.RentStatusEnum.Partially_paid) &&
                                                                                    r.TenantId == tenant.TenantId &&
                                                                                    (r.NextDateToPay >= DateTimeOffset.Now && r.NextDateToPay <= DateTimeOffset.Now.AddDays(2))
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

                                if (RentNotPaid.Count > 0)
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
                                        await ContructMessage(RentsThatNeedToBePay, TenantInfos, landLordinfos);
                                    }
                                }

                                //Locataire qui doit bientôt payer le loyer
                                //Cela ve dire que la fin du moi est proche donc il faut deja predir 
                                //la date pour le prochain mois
                                if (RentToPaySoon.Count != 0)
                                {
                                    foreach (var rent in RentToPaySoon)
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
                                            NextDateToPay = DateTime.UtcNow.AddMonths(1).AddDays(2) //The 2 days added is before we are sending reminders 2 days to the tenant before the actual date
                                        };
                                        _context.RentPaymentDatesSchedulars.Add(rentPaymentDatesSchedular);
                                        await _context.SaveChangesAsync();
                                    }
                                    await ContructMessage(RentToPaySoon, TenantInfos, landLordinfos);
                                }
                            }
                        }
                    }
                    else
                    {
                        string subject = "Urgent";
                        string body = $"<p>We cannot find information on the landlord who owns <p>\n";
                        body += $"<p>apartment number {LandLord.ApartmentNumber} belonging to the city called {LandLord.City.Name}";
                        body += "<p>Please solve this problem as soon as possible<p>";

                        await SendConfirmationEmail("pablodjeufoue@gmail.com", subject, body);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task EmailErrorMessage(string tenantEmailOrPhone, string? toLandlord = null)
        {
            if (String.IsNullOrWhiteSpace(toLandlord))
            {
                string ErrorSubject = "Urgent";
                string ErroBody = $"<p>Error when sending message to the tenant {tenantEmailOrPhone} to remind him to pay his rent <p>\n";
                ErroBody += "<p>Please solve this problem as soon as possible<p>";

                if (await SendConfirmationEmail("pablodjeufoue@gmail.com", ErrorSubject, ErroBody) == false)
                {
                    Console.WriteLine("Error while sending message to pablodjeufoue@gmail.com");
                }
            }
            else
            {
                string ErrorSubject = "Urgent";
                string ErroBody = $"<p>Error when sending message to the landlord {tenantEmailOrPhone}<p>\n";
                ErroBody += "<p>to also remind about tenant that need to pay the rent</p>";
                ErroBody += "<p>Please solve this problem as soon as possible<p>";

                if (await SendConfirmationEmail("pablodjeufoue@gmail.com", ErrorSubject, ErroBody) == false)
                {
                    Console.WriteLine("Error while sending message to pablodjeufoue@gmail.com");
                }
            }
        }

        public static string ConvertSmsRentsToString(List<(long ApartmentNumber, DateTimeOffset DateToPay, decimal AmountToPay)> rents)
        {
            string result = "";

            foreach (var rent in rents)
            {
                result += $"{rent.ApartmentNumber} - {String.Format("{0:dddd, MMMM d, yyyy}", rent.DateToPay)} - {rent.AmountToPay.ToString("0.00")} FCFA\n\n";
            }
            return result;
        }

        public async Task ContructMessage(List<RentPaymentDatesSchedular> rentsThatNeedToPay, ApplicationUser tenantInfos, ApplicationUser landLordinfos)
        {
            try
            {
                bool tenantEmailResponse = false;
                bool tenantSmsResponse = false;
                bool emailSentToLandlord = false;
                bool smsSendToLandlord = false;
                List<(long ApartmentNumber, DateTimeOffset DateToPay, decimal AmountToPay)> rentsToPay = new List<(long, DateTimeOffset, decimal)>();

                string subject = "Rappel de paiement de loyer";
                string body = "Loyers à payer:\n\n";
                string smsBody = "Loyers à payer:\n\n";


                body += "<table style=\"width:100%\">";
                body += "<thead>";
                body += "<tr>";
                body += "<th>Numéro d'appartement</th>";
                body += "<th>Date de paiement du loyer</th>";
                body += "<th>Montant à payer</th>";
                body += "</tr>";
                body += "</thead>";
                body += "<tbody>";

                List<NotificationSentForRentPayment> notificationSentForRentPayments = new List<NotificationSentForRentPayment>();

                foreach (var dt in rentsThatNeedToPay)
                {
                    body += "<tr>";
                    body += $"<td>{dt.ApartmentNumber}</td>";
                    body += $"<td>{String.Format("{0:dddd, MMMM d, yyyy}", dt.NextDateToPay)}</td>";
                    body += $"<td>{dt.AmmountSupposedToPay} FCFA</td>";
                    body += "</tr>";

                    //Sms body
                    rentsToPay.Add((dt.ApartmentNumber, dt.NextDateToPay, dt.AmmountSupposedToPay));

                    notificationSentForRentPayments.Add(new NotificationSentForRentPayment
                    {
                        CityId = dt.CityId,
                        ApartmentNumber = dt.ApartmentNumber,
                        TenantId = dt.TenantId,
                        AmmountSupposedToPay = dt.AmmountSupposedToPay,
                        ScheduledDateForRentPayment = dt.NextDateToPay.DateTime,
                        NotificationSentDate = DateTime.UtcNow
                    });
                }

                body += "</tbody>";
                body += "</table>";
                body += "<br>";
                body += "<br>";
                body += "<span style=\"font-weight:bold\">NB</span>: Les <span style=\"color:blue;font-weight:bold;font-style:italic\">Date de paiement du loyer</span> représentent les dates auxquelles vous deviez payer votre loyer</p>";


                smsBody += ConvertSmsRentsToString(rentsToPay);
                //End of sms body
                smsBody += "NB: Les dates représentent les dates auxquelles vous devez payer votre loyer.\n";
                smsBody += "Et le chiffre au début représente le numéro de votre appartement";

                if (!String.IsNullOrEmpty(tenantInfos.Email) && !String.IsNullOrEmpty(tenantInfos.PhoneNumber))
                {
                    //To Do: Need to pay Orange Api sms service per month
                    tenantSmsResponse = await sendSMStoTenant(tenantInfos.PhoneNumber, smsBody);
                    if (tenantSmsResponse == false)
                        await EmailErrorMessage(tenantInfos.Email);


                    tenantEmailResponse = await SendConfirmationEmail(tenantInfos.Email, subject, body);
                    if (tenantEmailResponse == false)
                        await EmailErrorMessage(tenantInfos.Email);
                }
                else if (!String.IsNullOrEmpty(tenantInfos.Email) && String.IsNullOrEmpty(tenantInfos.PhoneNumber))
                {
                    tenantEmailResponse = await SendConfirmationEmail(tenantInfos.Email, subject, body);

                    if (tenantEmailResponse == false)
                        await EmailErrorMessage(tenantInfos.Email);
                }

                //Send an email to the landlord of this appartment                        
                //To do: Modifier le tritre de cette notification
                subject = $"Loyer non payé par le locataire {tenantInfos.FirstName} {tenantInfos.LastName}";

                emailSentToLandlord = await SendConfirmationEmail(landLordinfos.Email, subject, body);
                if (emailSentToLandlord == false)
                    await EmailErrorMessage(landLordinfos.Email);

                smsSendToLandlord = await sendSMStoTenant(landLordinfos.PhoneNumber, smsBody);
                if (smsSendToLandlord == false)
                    await EmailErrorMessage(landLordinfos.Email);

                //Check if the notification has been sent before recording it as being sent
                if (tenantEmailResponse == true || tenantSmsResponse == true)
                {
                    await _context.AddRangeAsync(notificationSentForRentPayments);
                    await _context.SaveChangesAsync();
                }
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
