using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.Schedular;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System.Net;

namespace Logement.Controllers
{
    public class TenantController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<T> _logger;
        private BaseScheduler baseScheduler;
        long contractId;

        public TenantController(ApplicationDbContext context, IConfiguration configuration, 
            ILogger<T> logger, Services.EmailService emailService, UserManager<ApplicationUser> userManager,
                Services.SMSservice smsService)
            :base(context, configuration) 
        {
            _userManager = userManager;
            _logger = logger;
            baseScheduler = new BaseScheduler(context, logger, emailService, userManager, smsService);
        }

        private Apartment AddApartmenFromViewModel(ApartmentViewModel a)
        {
            Apartment apartment = new Apartment()
            {
                Id = a.Id,
                ApartmentNumber = a.ApartmentNunber,
                LessorId = a.LessorId,
                Description = a.Description,
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

        private TenantInfos GetTenantFromModel(CityMember tenant)
        {

            TenantInfos allTenant = new TenantInfos()
            {
                Id = tenant.Id,
                TenantEmail = tenant.User.Email,
                TenantPhoneNumber = tenant.User.PhoneNumber,
                ApartementPrice = tenant.Apartment.Price,
                DepositePrice = tenant.Apartment.DepositePrice,
            };        
            return allTenant;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTenants(long cityId)
        {
            try
            {
                List<TenantInfos> allTenants = new List<TenantInfos>();

                var landlord = await GetCityCreator(cityId);

                if (landlord == null)
                    return Forbid();

                var cityMembers = await dbc.CityMembers
                    .Where(cm => cm.CityId == cityId && cm.Role == CityMemberRoleEnum.Tenant)
                    .ToListAsync();
             
                foreach (var tenant in cityMembers)
                {
                    allTenants.Add(GetTenantFromModel(tenant));
                }
                ViewData["cityId"] = cityId;
                return View(allTenants);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task<IActionResult> AddApartment(ApartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                Apartment apartment = AddApartmenFromViewModel(model);
                var fileModel = new FileModel();

                //look for the user who is currently Login
                ApplicationUser user = dbc.Users
                                               .Where(u => u.UserName == GetUser().UserName)
                                               .First();

                apartment.LessorId = user.Id;

                apartment.CityId = (long)model.CityId;

                dbc.Apartments.Add(apartment);
                await dbc.SaveChangesAsync();

                foreach (var file in model.Images)
                {
                    string methodName = "AddApartment";
                    await SaveImageFile(file, apartment.Id, methodName);
                }
                return Redirect("/CityId/GetCities");
            }
            return View(model);
        }

        // Post: Admin/AddAsTenant/1
        [HttpGet]
        public async Task<IActionResult> AddAsTenant(long cityId, string email)
        {
            var city = await dbc.Cities.FindAsync(cityId);
            if (city == null)
                return BadRequest();

            CityMemberViewModel result = new CityMemberViewModel
            {
                TenantEmail = email,
                CityId = cityId
            }; 

            ViewData["cityId"] = cityId;
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsTenant(CityMemberViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AddApartment(model.AppartmentMember);

                    //Check if the user we are trying to add even exist
                    var user = await _userManager.FindByEmailAsync(model.TenantEmail);

                    if (user != null)
                    {
                        TenantRentApartment tenantRentApartment;
                        var fileModel = new FileModel();

                        await SaveFile(model.ContractFile, fileModel, user.Id);

                        tenantRentApartment = new TenantRentApartment()
                        {
                            TenantEmail = model.TenantEmail,
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

                        //For users having email
                        string emailSubject = $"<h4>Vous avez été ajouté comme locataire par Mr {GetUser().LastName} {GetUser().FirstName}.\n</h4>";
                        string emailBody = $"<p>Type de logement:{model.AppartmentMember.Type}</p><br>";
                        emailBody += $"<p>Localisation:{model.AppartmentMember.LocatedAt}</p>";
                        emailBody += $"<p>Superficie:{model.AppartmentMember.RoomArea}</p>";
                        emailBody += $"<p>Nombre de chambres:{model.AppartmentMember.NumberOfRooms}</p>";
                        emailBody += $"<p>Nombre de salle de bain:{model.AppartmentMember.NumberOfbathRooms}</p>";

                        //For users having phone number                              
                        string smsBody = $"Vous avez été ajouté comme locataire par Mr {GetUser().LastName} {GetUser().FirstName}.";
                        smsBody += $"Type de logement: {model.AppartmentMember.Type}";
                        smsBody += $"Localisation:{model.AppartmentMember.LocatedAt}";
                        smsBody += $"Superficie:{model.AppartmentMember.RoomArea}";
                        smsBody += $"Nombre de chambres:{model.AppartmentMember.NumberOfRooms}";
                        smsBody += $"Nombre de salle de bain:{model.AppartmentMember.NumberOfbathRooms}";

                        if (user.PhoneNumber != null && user.Email == null)
                            baseScheduler.sendSMStoTenant(user.PhoneNumber, smsBody);

                        else if (user.Email != null && user.PhoneNumber == null)
                            await baseScheduler.SendConfirmationEmail(user.Email, emailSubject, emailBody);

                        else if (user.Email != null && user.PhoneNumber != null)
                        {
                            baseScheduler.sendSMStoTenant(user.PhoneNumber, smsBody);
                            await baseScheduler.SendConfirmationEmail(user.Email, emailSubject, emailBody);
                        }

                        //Assign Tenant role to this user
                        await _userManager.AddToRoleAsync(user, "Tenant");

                        //Request to the database to find out the price on which the tenant and the lessor have agreed 
                        var apartmentPrice = await dbc.TenantRentApartments
                                                    .Where(t => t.TenantEmail == user.Email)
                                                    .Select(t => t.Price)
                                                    .FirstOrDefaultAsync();
                        decimal nbOfMonthPaid = 0;

                        nbOfMonthPaid = Decimal.Divide(model.AmountPaidByTenant, apartmentPrice);

                        //Add 30 days on the current payment date in case the amount paid is not enough
                        //Remind tenant after 30 days that he must pay his rent
                        PaymentHistory newPayment = new PaymentHistory()
                        {
                            TenantEmail = user.Email,
                            AmountPaid = model.AmountPaidByTenant,
                            NunberOfMonthPaid = nbOfMonthPaid.ToString(),
                            PaidDate = DateTime.UtcNow
                        };
                        dbc.PaymentHistories.Add(newPayment);
                        await dbc.SaveChangesAsync();


                        //Schedule the next date to pay the rent 
                        RentPaymentDatesSchedular rentPaymentDatesSchedular = new RentPaymentDatesSchedular
                        {
                            TenantEmail = user.Email,
                            IsRentPaidForThisDate = false,
                            AmmountSupposedToPay = apartmentPrice,
                            NextDateToPay = DateTimeOffset.UtcNow.AddMonths(Decimal.ToInt32(nbOfMonthPaid))
                        };
                        dbc.RentPaymentDatesSchedulars.Add(rentPaymentDatesSchedular);
                        await dbc.SaveChangesAsync();

                        //Need to remove this table
                        TenantPaymentStatu tenantPaymentStatus = new TenantPaymentStatu()
                        {
                            TenantEmail = user.Email,
                            NumberOfMonthsToPay = 0,
                            AmountRemainingForRent = 0,
                            RentStatus = Data.Enum.RentStatusEnum.Paid
                        };
                        dbc.TenantPaymentStatuses.Add(tenantPaymentStatus);
                        await dbc.SaveChangesAsync();

                        CityMember cityMember = new CityMember
                        {
                            CityId = (long)model.CityId,
                            UserId = user.Id,
                            Role = CityMemberRoleEnum.Tenant
                        };
                        dbc.CityMembers.Add(cityMember);
                        await dbc.SaveChangesAsync();

                        return Redirect("/Admin/GetAllTenants?cityId=" + model.CityId);
                    }
                    else
                        return BadRequest($"The user {model.TenantEmail} does not even exist as simple user");
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
            string IsRentPaid = !tenantRents.IsRentPaidForThisDate ? "No" : "Yes";
            DateTime dateToPay = tenantRents.NextDateToPay.DateTime;

            RentPaymentDatesSchedularViewModel allTenantRentInfos = new RentPaymentDatesSchedularViewModel()
            {
                Id = tenantRents.Id,
                TenantEmail = tenantRents.TenantEmail,
                AmmountSupposedToPay = tenantRents.AmmountSupposedToPay,
                IsRentPaidForThisDate = IsRentPaid,
                NextDateToPay = String.Format("{0:dddd, MMMM d, yyyy}", dateToPay)
            };
            return allTenantRentInfos;
        }

        [HttpGet]
        public  async Task<IActionResult> AllTenantRentStatus(string tenantEmail)
        {
            List<RentPaymentDatesSchedularViewModel> tenantRentStatus = new List<RentPaymentDatesSchedularViewModel>();


            var tenantRentInfos = await dbc.RentPaymentDatesSchedulars
                                           .Where(t => t.TenantEmail == tenantEmail)
                                           .ToListAsync();
        
            foreach(var infos in tenantRentInfos)
            {
                tenantRentStatus.Add(GetAllTenantInfos(infos));
            }
            return View(tenantRentStatus);
        }

        public PaymentHistoryViewModel GetPaymentFromModel(PaymentHistory payments)
        {
            PaymentHistoryViewModel allPayments = new PaymentHistoryViewModel()
            {
                Id = payments.Id,
                TenantEmail = payments.TenantEmail,
                AmountPaid = payments.AmountPaid,
                NunberOfMonthPaid = payments.NunberOfMonthPaid,
                PaidDate = payments.PaidDate
            };
            return allPayments;
        }

        public IActionResult GetAllPaymentsHistory(string userEmail)
        {
            List<PaymentHistoryViewModel> paymentHistory = new List<PaymentHistoryViewModel>();

            //Payments for a specific tenant
            var payments = dbc.PaymentHistories
                                .Where(t => t.TenantEmail == userEmail)
                                .ToList();
            if (payments != null)
            {
                foreach (PaymentHistory payment in payments)
                {
                    paymentHistory.Add(GetPaymentFromModel(payment));
                }
                return View(paymentHistory);
            }
            else
            {
                return NotFound("No paymentHistory for this user");
            }            
        }

        public async Task SaveFile(IFormFile formFile, FileModel fileModel, long? tenantId)
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
                        Name = fileName,
                        Size = formFile.Length
                    };
                    await formFile.CopyToAsync(fileStream);
                }
                fileModel.FileURL = $"/Admin/{nameof(GetFile)}?filename={fileName}";

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
