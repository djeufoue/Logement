using Logement.Data;
using Logement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Twilio.TwiML.Messaging;
using static NPOI.HSSF.Util.HSSFColor;

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

        public async Task RunSchedularMethod()
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

                var TenantInfos = await _userManager.FindByEmailAsync(tenant.TenantEmail);

                if (TenantInfos == null)
                    _logger.LogError($"User account {tenant.TenantEmail} does not exist!");
                else
                {
                    if (TenantInfos.Email == null && TenantInfos.PhoneNumber == null)
                    {
                        string subject = "Alerte";
                        string body = $"<p>Le locataire {TenantInfos.TenantFirstName} n'a pas d'e-mail ou de numéro de <p>\n";
                        body += "<p>téléphone pour lui rappeler de payer son loyer; ";
                        body += "<p>veuillez contacter l'administration pour résoudre ce problème<p>";

                        await SendConfirmationEmail(LandLord.Lessor.Email, subject, body);
                    }
                    else
                    {
                        DateTime currentDate = DateTime.UtcNow;

                        //Selectionner les dates dont le status est impaye pour ce locataire actif
                        List<RentPaymentDatesSchedular> RentNotPaid = await (from r in _context.RentPaymentDatesSchedulars
                                                                             where r.IsRentPaidForThisDate != true && r.TenantEmail == tenant.TenantEmail
                                                                             select new RentPaymentDatesSchedular
                                                                             {
                                                                                 TenantEmail = r.TenantEmail,
                                                                                 NextDateToPay = r.NextDateToPay,
                                                                                 AmmountSupposedToPay = r.AmmountSupposedToPay
                                                                             }).ToListAsync();

                        //Rents that need to be pay for this tenant
                        List<RentPaymentDatesSchedular> RentsThatNeedToPay = new List<RentPaymentDatesSchedular>();

                        //Ce locataire peut avoir un ou plusieurs appartements qu'il/elle loue
                        List<RentPaymentDatesSchedular> RentToPaySoon = new List<RentPaymentDatesSchedular>();

                        if (RentNotPaid != null)
                        {
                            foreach (var t in RentNotPaid)
                            {
                                var dateToPay = await _context.RentPaymentDatesSchedulars
                                                        .Where(r => r.TenantEmail == tenant.TenantEmail)
                                                        .Select(r => r.NextDateToPay)
                                                        .FirstAsync();


                                //Notification la plus rescente qui a été envoy pour rappeler ce locataire de payer son loyer:
                                var lastEmailSentDate = await _context.NotificationSentForRentPayments
                                                            .Where(t => t.TenantEmail == tenant.TenantEmail && t.ScheduledDateForRentPayment == dateToPay)
                                                            .OrderByDescending(d => d.NotificationSentDate)
                                                            .Select(t => t.NotificationSentDate)
                                                            .FirstOrDefaultAsync();

                                if (lastEmailSentDate != DateTime.MinValue)
                                {
                                    //Check if it's been more than 15 days since this tenant had to pay
                                    //This is for tenants with more than 15 days or more than a month's unpaid rent
                                    if ((lastEmailSentDate - currentDate).Days <= -15)
                                        RentsThatNeedToPay.Add(t);
                                }
                                //In the event that no notification was ever sent for this date of unpaid rent
                                else
                                {
                                    //Check if the person has to pay within the next two days or if the person had to pay but was not alerted when the time came
                                    if ((dateToPay - currentDate).Days >= 2 && (dateToPay - currentDate).Days < 2 || (dateToPay - currentDate).Days >= -4 && (dateToPay - currentDate).Days <=0)
                                    {
                                        RentToPaySoon.Add(t);
                                    }
                                }
                            }

                            if (RentsThatNeedToPay.Count != 0)
                            {
                                await ContructMessage(RentsThatNeedToPay, TenantInfos, tenant.TenantEmail);                                
                            }
                            //Locataire qui doit bientôt payer le loyer
                            if (RentToPaySoon.Count != 0)
                            {
                                await ContructMessage(RentToPaySoon, TenantInfos, tenant.TenantEmail);
                            }
                        }
                    }
                }
            }
        }

        public async Task ContructMessage(List<RentPaymentDatesSchedular> rentsThatNeedToPay, ApplicationUser tenantInfos, string? tenantEmail)
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
            body += "<th>Date de paiement du loyer</th>";
            body += "<th>Montant à payer</th>";
            body += "</tr>";
            foreach (var dt in rentsThatNeedToPay)
            {
                body += "<tr>";
                body += $"<td>{dt.NextDateToPay}</td>";
                body += $"<td>{dt.AmmountSupposedToPay}</td>";
                body += "<tr>";

                NotificationSentForRentPayment notificationSentForRentPayments = new NotificationSentForRentPayment
                {
                    TenantEmail = tenantEmail,
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

            if (tenantInfos.PhoneNumber != null && tenantInfos.Email == null)
                 sendSMStoTenant(tenantInfos.PhoneNumber, body);

            else if (tenantInfos.Email != null && tenantInfos.PhoneNumber == null)
                await SendConfirmationEmail(tenantInfos.Email, subject, body);

            else if (tenantInfos.Email != null && tenantInfos.PhoneNumber != null)
            {
                sendSMStoTenant(tenantInfos.PhoneNumber, body);
                await SendConfirmationEmail(tenantInfos.Email, subject, body);
            }

            //Send an email to the landlord of this appartment                        
            //To do: Modifier le tritre de cette notification
            subject = $"Loyer non payé par le locataire {tenantInfos.TenantFirstName} {tenantInfos.TenantLastName}";
            if (tenantInfos != null)
                await SendConfirmationEmail(tenantInfos.Email, subject, body);
        }
    }
}
