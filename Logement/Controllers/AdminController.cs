using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.Schedular;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using Newtonsoft.Json;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;

namespace Logement.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        protected readonly ILogger<T> _logger;
        private BaseScheduler baseScheduler;

        private string part;
        TenantRentApartment tenantRentApartment = new TenantRentApartment();
        private string GetPhoto;
        private string GetPart;
        long contractId;


        public AdminController(UserManager<ApplicationUser> userManager
            , ApplicationDbContext context, IConfiguration configuration,
            ILogger<T> logger,
            Services.EmailService emailService,
                Services.SMSservice smsService)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
            baseScheduler = new BaseScheduler(context, logger, emailService, userManager, smsService);
        }

        public async Task<JsonResult> CheckApartmentNumberAvailability(long apartmentNumber)
        {       
            var checkApartment = await _context.Apartments
                .Where( a => a.ApartmentNumber == apartmentNumber)
                .FirstOrDefaultAsync();

            if (checkApartment != null)
            {
                return Json(1);  //apartment number taken
            }
            else if (checkApartment == null)
                return Json(0); //apartment number not taken
            else 
                return Json(-1);  //error occurred
        }

        /// <summary>
        /// Collect informations about all the users and asign them to AllUsersViewModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private AllUsersViewModel GetViewModelFromModel(ApplicationUser user)
        {
            AllUsersViewModel allUsersViewModel = new AllUsersViewModel()
            {
                Id = user.Id,
                FirstName = user.TenantFirstName,
                LastName = user.TenantLastName,
                Email = user.Email,
                MaritalStatus = user.MaritalStatus,
                JobTitle = user.JobTitle,
                PhoneNumber = user.PhoneNumber
            };
            return allUsersViewModel;
        }


        private TenantRentApartmentViewModel GetTenantFromModel(TenantRentApartment tenant)
        {
            DateTime contractStartDate = tenant.StartOfContract;
            TenantRentApartmentViewModel allTenant = new TenantRentApartmentViewModel()
            {
                Id = tenant.Id,
                TenantEmail = tenant.TenantEmail,
                TenantPhoneNumber = tenant.TenantPhoneNumber,
                Price = tenant.Price,
                AmountPaidByTenant = tenant.AmountPaidByTenant,
                DepositePrice = tenant.DepositePrice,
                PaymentMethod = tenant.PaymentMethodEnum,
                StartOfContract = tenant.StartOfContract.ToLocalTime()
            };
            return allTenant;
        }

        /// <summary>
        ///Get the list of all apartments and pass them to the ApartmentViewModel before displaying them
        /// </summary>
        /// <param name="apartment"></param>
        /// <returns></returns>
        private ApartmentViewModel GetAllApartmentsFromModel(Apartment apartment, City city)
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel()
            {
                Id = apartment.Id,
                LessorId = apartment.LessorId,
                Description = apartment.Description,
                LocatedAt = city.LocatedAt,
                NumberOfRooms = apartment.NumberOfRooms,
                NumberOfbathRooms = apartment.NumberOfbathRooms,
                RoomArea = apartment.RoomArea,
                FloorNumber = apartment.FloorNumber,
                Price = apartment.Price,
                DepositePrice = apartment.DepositePrice,
                NumberOfParkingSpaces = city.NumberOfParkingSpaces,
                Status = apartment.Status,
                Type = apartment.Type,
                ImageURL = GetPhoto,
                Part = GetPart
            };
            return apartmentViewModel;
        }


        /// <summary>
        /// Use to add to collect informations about a new apartment that we want to add
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
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

        private CityViewModel GetCitiesFromModel(City city, string cityImage)
        {
            CityViewModel cityViewModel = new CityViewModel()
            {
                Id = city.Id,
                Name = city.Name,
                LocatedAt = city.LocatedAt,
                CityPhoto = cityImage,
                Floor = city.Floor,
                NumbersOfApartment = city.NumbersOfApartment,
                NumberOfParkingSpaces = city.NumberOfParkingSpaces
            };
            return cityViewModel;
        }

        private City AddCityFromViewModel(CityViewModel c)
        {
            City city = new City()
            {
                Id = c.Id,
                Name = c.Name,
                LocatedAt = c.LocatedAt,
                NumbersOfApartment = c.NumbersOfApartment,
                Floor = c.Floor,
                NumberOfParkingSpaces = c.NumberOfParkingSpaces,
                DateAdded = DateTime.UtcNow
            };
            return city;
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {          
            try
            {
                List<CityViewModel> citiesModel = new List<CityViewModel>();

                var cities = await _context.Cities.ToListAsync();

                if (cities != null)
                {
                    foreach (var city in cities)
                    {
                        var cityImage = await _context.CityPhotos
                            .Where(p => p.CityId == city.Id)
                            .Select(p => p.ImageURL)
                            .FirstOrDefaultAsync();

                        citiesModel.Add(GetCitiesFromModel(city, cityImage));
                    }
                }
                return View(citiesModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCityApartments(long cityId)
        {
            try
            {
                var currentUser = await _context.Users.FirstOrDefaultAsync();

                if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
                {
                    List<ApartmentViewModel> apartmentViewModel = new List<ApartmentViewModel>();

                    var apartmentList = await _context.Apartments
                        .Where(a => a.CityId == cityId)
                        .ToListAsync();

                    foreach (Apartment apartment in apartmentList)
                    {
                        GetApartmentPhoto(apartment.Id);

                        var city = await _context.Cities
                            .Where(c => c.Id == cityId)
                            .FirstOrDefaultAsync();

                        apartmentViewModel.Add(GetAllApartmentsFromModel(apartment, city));
                    }
                    return View(apartmentViewModel);
                }
                else
                    return Forbid("Only Admin or System admin can have access to this page");
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        //Get: Admin/AddApartment
        [HttpGet]
        public IActionResult AddCity()
        {
            CityViewModel city = new CityViewModel();
            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel cityViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    City city = AddCityFromViewModel(cityViewModel);
                    var fileModel = new FileModel();

                    //look for the user who is currently Login
                    ApplicationUser user = _context.Users
                                                   .Where(u => u.UserName == User.Identity.Name)
                                                   .First();


                    if (user != null)
                        city.LandLordId = user.Id;
                    else
                        return BadRequest("Please log out and log in back");

                    _context.Add(city);
                    await _context.SaveChangesAsync();

                    //Assign that city's id to the apartmentId Foreign Key which is inside the ApartmentPhoto Table

                    //Will be remove to use the methode down with the foreach lop
                    string methodName = "AddCity";
                    await SaveImageFile(cityViewModel.CityImage, city.Id, null, methodName);

                    //Will be use later 
                    /*  foreach (var photo in cityViewModel.CityPhotos)
                      {
                          string methodName = "AddCity";
                          await SaveImageFile(photo.ImageURL, city.Id, null, methodName);
                      }*/
                    return RedirectToAction(nameof(GetCities));
                }
                return View(cityViewModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        //To be removed
        //Get: Apartment
        [HttpGet]
        public async Task<IActionResult> ApartmentList()
        {       
            try
            {
                List<Apartment> apartments = await _context.Apartments.ToListAsync();
                List<ApartmentViewModel> apartmentViewModels = new List<ApartmentViewModel>();

                foreach (Apartment apartment in apartments)
                {
                    GetApartmentPhoto(apartment.Id);

                    var city = await _context.Cities
                        .Where(c => c.Id == apartment.CityId)
                        .FirstOrDefaultAsync();

                    apartmentViewModels.Add(GetAllApartmentsFromModel(apartment, city));
                }

                return View(apartmentViewModels);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                List<ApplicationUser> users = _userManager.Users.ToList();
                List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();
                foreach (ApplicationUser user in users)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        continue;

                    allUsersViewModels.Add(GetViewModelFromModel(user));
                }
                return View(allUsersViewModels);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllTenants()
        {
            try
            {
                List<TenantRentApartment> tenants = _context.TenantRentApartments.ToList();
                List<TenantRentApartmentViewModel> allTenantsViewModels = new List<TenantRentApartmentViewModel>();
                foreach (TenantRentApartment tenant in tenants)
                    allTenantsViewModels.Add(GetTenantFromModel(tenant));

                return View(allTenantsViewModels);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        /// <summary>
        /// Get the apartment photo and part
        /// </summary>
        /// <param name="apartmentId"></param>
        /// <returns></returns>
        public void GetApartmentPhoto(long apartmentId)
        {
            List<ApartmentPhoto> getapartmentPhotos = _context.ApartmentPhotos.ToList();

            foreach (ApartmentPhoto apartmentPhoto in getapartmentPhotos)
            {
                if (apartmentPhoto.ApartmentId == apartmentId)
                {
                    GetPhoto = apartmentPhoto.ImageURL;
                    GetPart = apartmentPhoto.Part;
                    break;
                }
            }
        }

        [AllowAnonymous]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string filePathName = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles") + fileName;

                    if (System.IO.File.Exists(filePathName))
                    {
                        var modificationDate = System.IO.File.GetLastWriteTimeUtc(filePathName);
                        FileStream fileStream = System.IO.File.OpenRead(filePathName);
                        string contentType = MimeTypes.GetMimeType(fileName);

                        var contenDisposotion = new System.Net.Mime.ContentDisposition()
                        {
                            FileName = fileName,
                            Inline = true,
                            Size = fileStream.Length,
                            ModificationDate = modificationDate
                        };
                        Response.Headers.Add("Content-Disposition", contenDisposotion.ToString());

                        return File(fileStream, contentType);
                    }
                    else return NotFound($"File '{fileName}' not found");
                }
                else return BadRequest("File name not Provided");
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task SaveImageFile(IFormFile formFile, long Id, string? photoPart, string methodName)
        {
            if (formFile != null)
            {
                string? folder = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles");
                string dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
                string fileName = $" ApartmentList {dateTime} {formFile.FileName}";

                using (FileStream fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                if (methodName == "AddApartment")
                {
                    ApartmentPhoto apartmentPhoto = new ApartmentPhoto()
                    {
                        ApartmentId = Id,
                        ImageURL = $"/Admin/{nameof(GetFile)}?filename={fileName}",
                        Part = photoPart
                    };
                    _context.Add(apartmentPhoto);
                    await _context.SaveChangesAsync();
                }
                else if (methodName == "AddCity")
                {
                    CityPhoto cityPhoto = new CityPhoto()
                    {
                        CityId = Id,
                        Size = formFile.Length,
                        ImageURL = $"/Admin/{nameof(GetFile)}?filename={fileName}"
                    };
                    _context.Add(cityPhoto);
                    await _context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Use to save File path into FileURL Colunm
        /// </summary>
        /// <param name="formFile"></param>
        /// <param name="fileModel"></param>
        /// <returns></returns>
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

                _context.Add(fileModel);
                await _context.SaveChangesAsync();

                //The file Url is unique because of the time which can't be the same for another file
                var contract = await _context.FileModel
                                       .Where(f => f.Name == fileName)
                                       .FirstOrDefaultAsync();
                //contract can't be null
                contractId = contract.Id;
            }
        }

        //Get: Admin/AddApartment
        [HttpGet]
        public async Task<IActionResult> AddApartment()
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel();
            List<CityViewModel> cityViewModel = new List<CityViewModel>();
            apartmentViewModel.PhotoSlots = new List<ApartmentPhotoViewModel>();

            var cities = await _context.Cities.ToListAsync();

            foreach (var city in cities)
            {
                CityViewModel cityV = new CityViewModel()
                {
                    Id = city.Id,
                    Name = city.Name
                };
                cityViewModel.Add(cityV);
            }

            if (cityViewModel != null)
            {
                foreach (var city in cityViewModel)
                    apartmentViewModel.Cities.Add(city);
            }
            return View(apartmentViewModel);
        }

        public async Task<IActionResult> AddApartment(ApartmentViewModel apartmentViewModel)
        {
            if (ModelState.IsValid)
            {
                Apartment apartment = AddApartmenFromViewModel(apartmentViewModel);
                var fileModel = new FileModel();

                //look for the user who is currently Login
                ApplicationUser user = _context.Users
                                               .Where(u => u.UserName == User.Identity.Name)
                                               .First();

                apartment.LessorId = user.Id;

                if (apartmentViewModel.CityIds != null)
                    apartment.CityId = apartmentViewModel.CityIds.FirstOrDefault();

                _context.Add(apartment);
                await _context.SaveChangesAsync();

                apartmentViewModel.PhotoSlots.Add(apartmentViewModel.apartmentPhotoViewModel);

                foreach (var photo in apartmentViewModel.PhotoSlots)
                {
                    string methodName = "AddApartment";
                    await SaveImageFile(photo.ImageURL, apartment.Id, photo.Part, methodName);
                }
                return RedirectToAction(nameof(ApartmentList));
            }
            return View(apartmentViewModel);
        }


        // Post: Admin/AddAsTenant/1
        [HttpGet]
        public async Task<IActionResult> AddAsTenantAsync(long tenantId, string email)
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel();
            List<CityViewModel> cityViewModel = new List<CityViewModel>();
            apartmentViewModel.PhotoSlots = new List<ApartmentPhotoViewModel>();

            var cities = await _context.Cities.ToListAsync();

            foreach (var city in cities)
            {
                CityViewModel cityV = new CityViewModel()
                {
                    Id = city.Id,
                    Name = city.Name
                };
                cityViewModel.Add(cityV);
            }

            if (cityViewModel != null)
            {
                foreach (var city in cityViewModel)
                    apartmentViewModel.Cities.Add(city);
            }

            TenantRentApartmentViewModel result = new TenantRentApartmentViewModel
            {
                TenantId = tenantId,
                TenantEmail = email,
                TenantApartment = apartmentViewModel
            };
            return View(result);
        }


        //To Do: Try to get the right bailId, and change the status of the apartment from free to basy.
        [HttpPost]
        public async Task<IActionResult> AddAsTenant(TenantRentApartmentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await AddApartment(model.TenantApartment);                  

                    ApplicationUser landLord = _context.Users
                                          .Where(u => u.UserName == User.Identity.Name)
                                        .First();
                    if (landLord != null)
                    {
                        var user = await _userManager.Users
                                        .Where(u => u.Id == model.TenantId)
                                        .FirstOrDefaultAsync();

                        if (user != null)
                        {
                            TenantRentApartment tenantRentApartment;
                            var fileModel = new FileModel();

                            await SaveFile(model.ContractFile, fileModel, user.Id);

                            tenantRentApartment = new TenantRentApartment()
                            {
                                TenantEmail = model.TenantEmail,
                                TenantPhoneNumber = user.PhoneNumber,
                                ApartmentId = model.TenantApartment.Id,
                                Price = model.Price,
                                BailId = contractId,
                                AmountPaidByTenant = model.AmountPaidByTenant,
                                DepositePrice = model.DepositePrice,
                                PaymentMethodEnum = model.PaymentMethod,
                                StartOfContract = model.StartOfContract,
                                EndOfContract = model.EndOfContract,
                                IsActiveAsTenant = true
                            };

                            _context.Add(tenantRentApartment);
                            await _context.SaveChangesAsync();

                            //For users having email
                            string emailSubject = $"<h4>Vous avez été ajouté comme locataire par Mr {landLord.TenantLastName} {landLord.TenantFirstName}.\n</h4>";
                            string emailBody = $"<p>Type de logement:{model.TenantApartment.Type}</p><br>";
                            emailBody += $"<p>Localisation:{model.TenantApartment.LocatedAt}</p>";
                            emailBody += $"<p>Superficie:{model.TenantApartment.RoomArea}</p>";
                            emailBody += $"<p>Nombre de chambres:{model.TenantApartment.NumberOfRooms}</p>";
                            emailBody += $"<p>Nombre de salle de bain:{model.TenantApartment.NumberOfbathRooms}</p>";

                            //For user having phone number                              
                            string smsBody = $"Vous avez été ajouté comme locataire par Mr {landLord.TenantLastName} {landLord.TenantFirstName}.";
                            smsBody += $"Type de logement: {model.TenantApartment.Type}";
                            smsBody += $"Localisation:{model.TenantApartment.LocatedAt}";
                            smsBody += $"Superficie:{model.TenantApartment.RoomArea}";
                            smsBody += $"Nombre de chambres:{model.TenantApartment.NumberOfRooms}";
                            smsBody += $"Nombre de salle de bain:{model.TenantApartment.NumberOfbathRooms}";

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
                            var apartmentPrice = await _context.TenantRentApartments
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
                            _context.Add(newPayment);
                            await _context.SaveChangesAsync();


                            //Schedule the next date to pay the rent 
                            RentPaymentDatesSchedular rentPaymentDatesSchedular = new RentPaymentDatesSchedular
                            {
                                TenantEmail = user.Email,
                                IsRentPaidForThisDate = false,
                                AmmountSupposedToPay = apartmentPrice,
                                NextDateToPay = DateTimeOffset.UtcNow.AddMonths(Decimal.ToInt32(nbOfMonthPaid))
                            };
                            _context.Add(rentPaymentDatesSchedular);
                            await _context.SaveChangesAsync();

                            //Need to remove this table
                            TenantPaymentStatu tenantPaymentStatus = new TenantPaymentStatu()
                            {
                                TenantEmail = user.Email,
                                NumberOfMonthsToPay = 0,
                                AmountRemainingForRent = 0,
                                RentStatus = Data.Enum.RentStatusEnum.Paid
                            };
                            _context.Add(tenantPaymentStatus);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(GetAllTenants));
                        }
                        else
                            return BadRequest($"The user {model.TenantEmail} does not even exist as simple user");
                    }
                    else
                        return BadRequest("please log in");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
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

        //To do: implement this page
        [HttpGet]
        public IActionResult AllMyTenant()
        {
            return View();
        }
    }
}
