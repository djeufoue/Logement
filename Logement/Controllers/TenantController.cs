﻿using iTextSharp.text.pdf;
using iTextSharp.text;
using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.Schedular;
using Logement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Net;
using Twilio.TwiML.Voice;
using Logement.DTO;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Dml.Diagram;
using Microsoft.AspNetCore.Authorization;
using Logement.Helpers;

namespace Logement.Controllers
{
    [Authorize]
    public class TenantController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<T> _logger;
        private BaseScheduler baseScheduler;
        long contractId;

        public TenantController(ApplicationDbContext context, IConfiguration configuration,
            ILogger<T> logger, Services.EmailService emailService, UserManager<ApplicationUser> userManager,
                Services.SMSservice smsService, TenancyHelpers tenancyHelpers)
            : base(context, configuration)
        {
            _userManager = userManager;
            _logger = logger;
            baseScheduler = new BaseScheduler(context, logger, emailService, userManager, smsService);
        }

        [HttpPost]
        public async Task<IActionResult> CheckMultipleImageExistence(List<IFormFile> files)
        {
            try
            {
                List<string> existingFiles = new List<string>();

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await file.CopyToAsync(ms);
                            byte[] imageData = ms.ToArray();

                            var checkImage = await dbc.Fichiers
                                .Where(f => f.Data.SequenceEqual(imageData))
                                .FirstOrDefaultAsync();

                            if (checkImage != null)
                            {
                                existingFiles.Add(file.FileName);
                            }
                        }
                    }
                }

                if (existingFiles.Count > 0)
                {
                    return Json(existingFiles);
                }
                else
                {
                    return Json(""); 
                }
            }
            catch (Exception)
            {
                return Json(-1); // Error occurred
            }
        }

        private Apartment AddApartmenFromViewModel(ApartmentViewModel a)
        {
            Apartment apartment = new Apartment()
            {
                Id = a.Id,
                ApartmentNumber = a.ApartmentNunber,
                LessorId = a.LessorId,
                NumberOfRooms = a.NumberOfRooms,
                NumberOfbathRooms = a.NumberOfbathRooms,
                RoomArea = a.RoomArea,
                FloorNumber = a.FloorNumber,
                Price = a.Price,
                DepositePrice = a.DepositePrice,
                Status = a.Status,
                CreatedOn = DateTime.UtcNow,
                Type = a.Type,
            };
            return apartment;
        }

        public async Task<IActionResult> AddApartment(ApartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                Apartment apartment = AddApartmenFromViewModel(model);

                var cityCreator = await GetCityCreator(model.CityId);

                if (cityCreator == null)
                    return Forbid();

                apartment.LessorId = cityCreator.User.Id;

                apartment.CityId = (long)model.CityId;

                dbc.Apartments.Add(apartment);
                await dbc.SaveChangesAsync();

                foreach (var file in model.Images)
                {
                    string methodName = "AddApartment";
                    await SaveImageFile(file, apartment.Id, methodName);
                }
                model.Id = apartment.Id;
                return Redirect("/Apartment/Index?cityId=" + model.CityId);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddAsTenant(long cityId, long tenantId)
        {
            var city = await dbc.Cities.FindAsync(cityId);
            if (city == null)
                return BadRequest();

            CityMemberViewModel result = new CityMemberViewModel
            {
                TenantId = tenantId,
                AppartmentMember = new ApartmentViewModel
                {
                    CityId = cityId,
                    LocatedAt = city.LocatedAt
                }
            };

            ViewData["cityId"] = cityId;
            ViewData["locatedAt"] = city.LocatedAt;
            ViewData["tenantId"] = tenantId;
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsTenantV2(TenancyMemberDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = GetCurrentUser();

                    if (currentUser == null)
                        return StatusCode((int)HttpStatusCode.NotFound, "The current user is not logged in or does not exist");

                    if (currentUser == null)
                        return StatusCode((int) HttpStatusCode.NotFound, "The current user is not logged in or does not exist");

                    var user = await dbc.Users.Where(u => u.Id == model.TenantId).FirstOrDefaultAsync();

                    if (user == null)
                        return StatusCode((int)HttpStatusCode.NotFound, "This user does not exist");

                    var tenancy = await dbc.Tenancies
                        .Include(t => t.Apartment).ThenInclude(t => t.City)
                        .Where(t => t.Id == model.TenancyId).FirstOrDefaultAsync();

                    if (tenancy == null)
                        return StatusCode((int)HttpStatusCode.NotFound, "This tenancy does not exist or was deleted");

                    var tenantMember = await dbc.TenancyMembers
                        .Where(tm => tm.TenancyId == tenancy.Id && tm.TenantId == user.Id)
                        .FirstOrDefaultAsync();

                    if (tenantMember != null)
                        return StatusCode((int)HttpStatusCode.BadRequest, "This user is already a tenant of this tenancy");

                    TenancyMemberRoleEnum tenantRole = BasicHelpers.GetTenancyMemberRole(model.Role);

                    tenantMember = new TenancyMember
                    {
                        TenancyId = tenancy.Id,
                        TenantId = user.Id,
                        AdderId = currentUser.Id,
                        DateAdded = DateTime.UtcNow,
                        Role = tenantRole,
                        SendEmail = model.SendEmail
                    };

                    dbc.TenancyMembers.Add(tenantMember);
                    await dbc.SaveChangesAsync();

                    // Generate personalized message contents
                    string recipientName = BasicHelpers.GetUserFullName(user);
                    string currentUserFullName = BasicHelpers.GetUserFullName(currentUser);

                    var (emailSubject, emailBody) = TenancyHelpers.GenerateEmailContent(tenancy, currentUserFullName, recipientName);
                    string smsBody = TenancyHelpers.GenerateSmsContent(tenancy, currentUserFullName);

                    if (!string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(user.PhoneNumber))
                    {
                        await baseScheduler.sendSMStoTenant(user.PhoneNumber, smsBody);
                        await baseScheduler.SendEmail(user.Email, emailSubject, emailBody);
                    }
                    else if (!string.IsNullOrEmpty(user.Email))
                    {
                        await baseScheduler.SendEmail(user.Email, emailSubject, emailBody);
                    }                
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsTenant(CityMemberViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = GetCurrentUser();

                    if (currentUser == null)
                        return StatusCode((int)HttpStatusCode.NotFound, "The current user is not logged in or does not exist");

                    var checkApartmentNumber = await dbc.Apartments
                        .Where(ap => ap.CityId == model.AppartmentMember.CityId
                        && ap.ApartmentNumber == model.AppartmentMember.ApartmentNunber)
                        .FirstOrDefaultAsync();

                    if (checkApartmentNumber != null)
                        return BadRequest();

                    await AddApartment(model.AppartmentMember);

                    //Check if the user we are trying to add even exist
                    var user = await _userManager.FindByIdAsync(model.TenantId.ToString());

                    if (user == null)
                        return BadRequest($"The user does not even exist");

                    TenantRentApartment tenantRentApartment;
                    var fileModel = new FileModel();

                    await SaveFile(model.ContractFile, fileModel, user.Id, model.AppartmentMember.CityId, model.AppartmentMember.Id);

                    tenantRentApartment = new TenantRentApartment()
                    {
                        TenantId = (long)model.TenantId,
                        TenantPhoneNumber = user.PhoneNumber,
                        ApartmentId = model.AppartmentMember.Id,
                        Price = model.AppartmentMember.Price,
                        BailId = contractId,
                        AmountPaidByTenant = model.AmountPaidByTenant,
                        DepositePrice = model.DepositePrice,
                        PaymentMethodEnum = model.PaymentMethod,
                        StartOfContract = model.StartOfContract,
                        EndOfContract = model.EndOfContract,
                        IsActiveAsTenant = true
                    };

                    dbc.TenantRentApartments.Add(tenantRentApartment);
                    await dbc.SaveChangesAsync();

                    string propertyName = await dbc.Cities
                        .Where(c => c.Id == model.AppartmentMember.CityId)
                        .Select(c => c.Name)
                        .FirstOrDefaultAsync();

                    // Generate personalized message contents
                    string recipientName = BasicHelpers.GetUserFullName(user);
                    string currentUserFullName = BasicHelpers.GetUserFullName(currentUser);

                    var (emailSubject, emailBody) = TenancyHelpers.GenerateEmailContent(model, currentUserFullName, recipientName);
                    string smsBody = TenancyHelpers.GenerateSmsContent(model, currentUserFullName);

                    if (!String.IsNullOrEmpty(user.Email) && !String.IsNullOrEmpty(user.PhoneNumber))
                    {
                        //To Do: Need to pay Orange Api sms service per month
                        await baseScheduler.sendSMStoTenant(user.PhoneNumber, smsBody);
                        await baseScheduler.SendEmail(user.Email, emailSubject, emailBody);
                    }
                    else if (!String.IsNullOrEmpty(user.Email) && String.IsNullOrEmpty(user.PhoneNumber))
                        await baseScheduler.SendEmail(user.Email, emailSubject, emailBody);

                    decimal nbOfMonthPaid = 0;

                    nbOfMonthPaid = Decimal.Divide(model.AmountPaidByTenant, model.AppartmentMember.Price);
                
                    //Add 30 days on the current payment date in case the amount paid is not enough
                    //Remind tenant after 30 days that he must pay his rent
                    Logement.Models.PaymentHistory newPayment = new Logement.Models.PaymentHistory()
                    {
                        TenantId = user.Id,
                        CityId = model.AppartmentMember.CityId,
                        ApartmentNumber = model.AppartmentMember.ApartmentNunber,
                        AmountPaid = model.AmountPaidByTenant,
                        PaidDate = DateTime.UtcNow
                    };
                    dbc.PaymentHistories.Add(newPayment);
                    await dbc.SaveChangesAsync();

                    decimal extraAmount = model.AmountPaidByTenant - (model.AppartmentMember.Price * Math.Floor(nbOfMonthPaid));

                    //Schedule the next date to pay the rent 
                    RentPaymentDatesSchedular rentPaymentDatesSchedular = new RentPaymentDatesSchedular
                    {
                        TenantId = user.Id,
                        CityId = model.AppartmentMember.CityId,
                        ApartmentNumber = model.AppartmentMember.ApartmentNunber,
                        AmountAlreadyPaid = extraAmount > 0 ? extraAmount: 0,
                        RemainingAmount = model.AppartmentMember.Price,
                        RentStatus = extraAmount > 0 ? RentStatusEnum.Partially_paid : RentStatusEnum.Unpaid,
                        AmmountSupposedToPay = model.AppartmentMember.Price,
                        NextDateToPay = DateTime.UtcNow.AddMonths(Decimal.ToInt32(nbOfMonthPaid))
                    };
                    dbc.RentPaymentDatesSchedulars.Add(rentPaymentDatesSchedular);
                    await dbc.SaveChangesAsync();

                    CityMember cityMember = new CityMember
                    {
                        CityId = model.AppartmentMember.CityId,
                        ApartmentId = (long)model.AppartmentMember.Id,
                        UserId = user.Id,
                        Role = CityMemberRoleEnum.Tenant
                    };
                    dbc.CityMembers.Add(cityMember);
                    await dbc.SaveChangesAsync();

                    return Redirect("/Apartment/Index?cityId=" + model.AppartmentMember.CityId);                                        
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private RentPaymentDatesSchedularViewModel GetAllTenantInfos(RentPaymentDatesSchedular tenantRents)
        {
            RentPaymentDatesSchedularViewModel allTenantRentInfos = new RentPaymentDatesSchedularViewModel()
            {
                Id = tenantRents.Id,
                TenantId = tenantRents.TenantId,
                CityId = tenantRents.CityId,
                ApartmentNumber = tenantRents.ApartmentNumber,
                TenantFullName = $"{tenantRents.Tenant.FirstName} {tenantRents.Tenant.LastName}",
                AmmountSupposedToPay = tenantRents.AmmountSupposedToPay,
                AmountAlreadyPaid = tenantRents.AmountAlreadyPaid,
                RemainingAmount = tenantRents.RemainingAmount,
                RentStatus = tenantRents.RentStatus,
                ExpectedDateToPay = String.Format("{0:dddd, MMMM d, yyyy}", tenantRents.NextDateToPay)
            };
            return allTenantRentInfos;
        }

        [HttpGet]
        public async Task<IActionResult> TenantRentStatus(long cityId, long tenantId, long apartmentMumber)
        {
            try
            {
                var cityOwner = await dbc.Cities
                    .Where(c => c.Id == cityId && c.LandLordId == GetCurrentUser().Id)
                    .FirstOrDefaultAsync();

                List<RentPaymentDatesSchedularViewModel> tenantRentStatus = new List<RentPaymentDatesSchedularViewModel>();

                DateTime currentDate = DateTime.UtcNow;

                //Unpaid or partially paid rent for this tenant
                var tenantRentInfos = await dbc.RentPaymentDatesSchedulars
                    .Where(t => t.TenantId == tenantId && t.ApartmentNumber == apartmentMumber &&
                                (t.RentStatus == RentStatusEnum.Unpaid || t.RentStatus == RentStatusEnum.Partially_paid) &&
                                currentDate > t.NextDateToPay)
                    .Include(t => t.Tenant)
                    .ToListAsync();

                foreach (var infos in tenantRentInfos)
                {
                    tenantRentStatus.Add(GetAllTenantInfos(infos));
                }

                var rentStatus = await dbc.RentPaymentDatesSchedulars
                     .Where(rp => rp.CityId == cityId && rp.TenantId == tenantId && rp.ApartmentNumber == apartmentMumber)
                     .OrderByDescending(rp => rp.NextDateToPay)
                     .FirstOrDefaultAsync();
              
                ViewData["cityOwner"] = cityOwner;
                ViewData["rentStatus"] = rentStatus;

                return View(tenantRentStatus);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PayRent(long tenantId, long rentId, decimal amount)
        {
            try
            {
                var cityOwner = await dbc.CityMembers
                    .Include(c => c.City)
                    .Where(c => c.UserId == GetCurrentUser().Id && c.Role == CityMemberRoleEnum.Landord)
                    .FirstOrDefaultAsync();

                if (cityOwner == null)
                    return NotFound("You are not the owner of the city");

                var tenant = await dbc.CityMembers.Where(c => c.UserId == tenantId).FirstOrDefaultAsync();

                if (tenant == null)
                    return NotFound("This user is not a tenant of this city");

                var rent = await dbc.RentPaymentDatesSchedulars
                    .Where(r => r.Id == rentId && r.TenantId == tenantId)
                    .FirstOrDefaultAsync();

                if (rent == null)
                    return NotFound("This specific rent was not found");

                rent.AmountAlreadyPaid += amount;

                if (rent.AmountAlreadyPaid >= rent.AmmountSupposedToPay)
                {
                    rent.RentStatus = RentStatusEnum.Paid;
                    rent.RemainingAmount = 0;
                }
                else if (rent.AmountAlreadyPaid < rent.AmmountSupposedToPay)
                {
                    rent.RentStatus = RentStatusEnum.Partially_paid;
                    rent.RemainingAmount = rent.AmmountSupposedToPay - rent.AmountAlreadyPaid;
                }
                dbc.Update(rent);
                await dbc.SaveChangesAsync();

                Logement.Models.PaymentHistory newPayment = new Logement.Models.PaymentHistory()
                {
                    TenantId = tenantId,
                    ApartmentNumber = rent.ApartmentNumber,
                    AmountPaid = amount,
                    PaidDate = DateTime.UtcNow
                };
                dbc.PaymentHistories.Add(newPayment);
                await dbc.SaveChangesAsync();

                return Redirect("/Tenant/TenantRentStatus?tenantId=" + tenantId);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public PaymentHistoryViewModel GetPaymentFromModel(Logement.Models.PaymentHistory payments)
        {
            PaymentHistoryViewModel allPayments = new PaymentHistoryViewModel()
            {
                Id = payments.Id,
                //CityId = payments.CityId,
                //CityName = payments.City.Name,
                TenantId = payments.Tenant.Id,
                ApartmentNumber = payments.ApartmentNumber,
                TenantFullName = $"{payments.Tenant.FirstName} {payments.Tenant.LastName}",
                AmountPaid = payments.AmountPaid,
                PaidDate = String.Format("{0:dddd, MMMM d, yyyy}", payments.PaidDate)
            };
            return allPayments;
        }

        //Add security
        [HttpGet]
        public IActionResult GetAllPaymentsHistory(long tenantId)
        {
            List<PaymentHistoryViewModel> paymentHistory = new List<PaymentHistoryViewModel>();

            //Payments for a specific tenant
            var payments = dbc.PaymentHistories
                                .Where(t => t.TenantId == tenantId)
                                .Include(t => t.Tenant)
                                //.Include( t => t.City)
                                .ToList();

            if (payments != null)
            {
                foreach (Logement.Models.PaymentHistory payment in payments)
                {
                    paymentHistory.Add(GetPaymentFromModel(payment));
                }
                return View(paymentHistory);
            }
            else
            {
                return NotFound("No paymentHistory for this tenant");
            }
        }

        public IActionResult DownloadReceipt(long paymentId)
        {
            try
            {
                // Retrieve the payment record
                var payment = dbc.PaymentHistories
                    .Include(p => p.Tenant)
                    .FirstOrDefault(p => p.Id == paymentId);

                if (payment == null)
                    return NotFound("Payment record not found");

                // Generate the receipt using iTextSharp
                var document = new Document();
                var memoryStream = new MemoryStream();
                var writer = PdfWriter.GetInstance(document, memoryStream);

                // Set PDF document properties
                document.AddTitle("Rent receipt");
                document.AddAuthor("DJE Rentals");
                document.AddSubject("Rent payment for this month");


                using (document)
                {
                    using (writer)
                    {
                        document.Open();

                        // Add content to the receipt
                        var fullName = $"{payment.Tenant.FirstName} {payment.Tenant.LastName}";
                        var receiptContent = $"Receipt for payment made by {fullName} on {payment.PaidDate}. Amount Paid: {payment.AmountPaid} FCFA";

                        document.Add(new Paragraph(receiptContent));

                        // Close the document
                        document.Close();
                        writer.Close();

                        // Prepare the file download response
                        var fileData = memoryStream.ToArray();

                        // Set the file content type and headers
                        var contentType = "application/pdf";
                        var fileName = $"Receipt_{paymentId}.pdf";

                        // Return the file as a download response
                        return File(fileData, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        public async System.Threading.Tasks.Task SaveFile(IFormFile formFile, FileModel fileModel, long? tenantId,long cityId, long apartmentId)
        {
            if (formFile != null)
            {
                string? folder = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles");
                string dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
                string fileName = $" ApartmentList {dateTime} {formFile.FileName}";

                using (FileStream fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
                {
                    fileModel = new FileModel()
                    {
                        TenantId = tenantId,
                        CityId = cityId,
                        ApartmentId = apartmentId,
                        Name = fileName,
                        Size = formFile.Length
                    };
                    await formFile.CopyToAsync(fileStream);
                }
                fileModel.FileURL = $"/Tenant{nameof(GetFile)}?filename={fileName}";

                dbc.FileModel.Add(fileModel);
                await dbc.SaveChangesAsync();

                //The file Url is unique because of the time which can't be the same for another file
                var contract = await dbc.FileModel
                                       .Where(f => f.Name == fileName)
                                       .FirstOrDefaultAsync();
                //contract can't be null
                contractId = contract.Id;
            }
        }

        protected ActionResult InternalServerError(Exception ex, string? message = null)
        {
            _logger.LogError(ex, message);
            if (message == null)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                message = ex.Message;
            }
            return StatusCode((int)HttpStatusCode.InternalServerError, message);
        }
    }
}
