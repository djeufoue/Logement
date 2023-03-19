using Logement.Data;
using Logement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using NPOI.SS.Formula.Functions;
using NPOI.XWPF.UserModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        public async Task RunSchedularMethod()
        {

            NotificationSentForRentPayment notificationSentForRentPayments;
            //Find all active tenants
            var tenantStatus = await _context.TenantRentApartments
                                       .Where(t => t.IsActiveAsTenant == true)
                                       .ToListAsync();

            //For each active tenant 
            foreach (var tenant in tenantStatus)
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

                if (RentNotPaid != null)
                {
                    foreach (var t in RentNotPaid)
                    {
                        var dateToPay = await _context.RentPaymentDatesSchedulars
                                                .Where(r => r.TenantEmail == t.TenantEmail)
                                                .Select(r => r.NextDateToPay)
                                                .FirstAsync();

                       
                        if ((dateToPay - currentDate).Days <= 15)
                        {
                            //Notification la plus rescente qui a été pour rappeler ce loyer de payer son loyer:
                            var lastEmailSentDate = await _context.NotificationSentForRentPayments
                                                        .Where(t => t.TenantEmail == tenant.TenantEmail)
                                                        .OrderByDescending(d => d.NotificationSentDate)
                                                        .Select( t => t.NotificationSentDate)
                                                        .FirstOrDefaultAsync();

                            //If true, sent another email to this tenant to remind him/her
                            if((lastEmailSentDate - currentDate).Days <= -15)
                                RentsThatNeedToPay.Add(t);
                        }
                    }

                    if (RentsThatNeedToPay.Count != 0)
                    {
                        //Le nonbre de mois impayé
                        int count = RentsThatNeedToPay.Count();

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
                        foreach (var dt in RentsThatNeedToPay)
                        {
                            body += "<tr>";
                            body += $"<td>{dt.NextDateToPay}</td>";
                            body += $"<td>{dt.AmmountSupposedToPay}</td>";
                            body += "<tr>";

                            notificationSentForRentPayments = new NotificationSentForRentPayment
                            {
                                TenantEmail = tenant.TenantEmail,
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


                        //To do: condition to check if we need to send an emait to the tenant or an sms 
                        //Send a Email to this tenant 
                        await SendConfirmationEmail(tenant.TenantEmail, body);

                        //Send a email to the landlord of this appartment 
                        var LandLord = await _context.Apartments
                                   .Where(a => a.Id == tenant.ApartmentId)
                                   .FirstOrDefaultAsync();

                        if (LandLord != null)
                        {
                            ApplicationUser? LandLordinfos = _context.Users.Find(LandLord.LessorId);
                            //Not suppose to be null 
                            //To do: Modifier le tritre de cette notification
                            if (LandLordinfos != null)
                                await SendConfirmationEmail(LandLordinfos.Email, body);
                        }
                    }
                }

                //Add Another Logic to remind tenant that need to pay within one days 
            }
        }
    }
}
