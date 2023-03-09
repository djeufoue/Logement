using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NPOI.SS.Formula.Functions;
using System.Net;

namespace Logement.Controllers
{
    [Authorize(Roles = "Admin,SystemAdmin")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        protected readonly ILogger<T> _logger;

        private string part;
        private ApartmentPhoto apartmentPhoto = new ApartmentPhoto();
        TenantRentApartment tenantRentApartment = new TenantRentApartment();
        private string  GetPhoto;
        private string GetPart;
        long contractId;


        public AdminController(UserManager<ApplicationUser> userManager
            ,ApplicationDbContext context, IConfiguration configuration, ILogger<T> logger)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
            _logger = logger;
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
            TenantRentApartmentViewModel allTenant = new TenantRentApartmentViewModel()
            {
                Id = tenant.Id,
                TenantEmail = tenant.TenantEmail,
                TenantPhoneNumber = tenant.TenantPhoneNumber,
                Price = tenant.Price,
                AmountPaidByTenant = tenant.AmountPaidByTenant,
                DepositePrice = tenant.DepositePrice,
                PaymentMethod = tenant.PaymentMethodEnum,              
                StartOfContract = tenant.StartOfContract,
            };
            return allTenant;
        }

        /// <summary>
        ///Get the list of all apartments and pass them to the ApartmentViewModel before displaying them
        /// </summary>
        /// <param name="apartment"></param>
        /// <returns></returns>
        private ApartmentViewModel GetAllApartmentsFromModel(Apartment apartment)
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel()
            {
                Id = apartment.Id,
                LessorId = apartment.LessorId,
                Description = apartment.Description,
                LocatedAt = apartment.LocatedAt,
                NumberOfRooms = apartment.NumberOfRooms,
                NumberOfbathRooms = apartment.NumberOfbathRooms,
                RoomArea = apartment.RoomArea,
                FloorNumber = apartment.FloorNumber,
                Price = apartment.Price,
                DepositePrice = apartment.DepositePrice,                           
                NumberOfParkingSpaces = apartment.NumberOfParkingSpaces,
                Status = apartment.Status,
                Type = apartment.Type,
                ImageURL = GetPhoto,
                Part = GetPart,
                CreatedOn = apartment.CreatedOn
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
                LessorId = a.LessorId,
                Description = a.Description,
                LocatedAt = a.LocatedAt,
                NumberOfRooms = a.NumberOfRooms,
                NumberOfbathRooms = a.NumberOfbathRooms,
                RoomArea = a.RoomArea,
                FloorNumber = a.FloorNumber,
                Price = a.Price,
                DepositePrice = a.DepositePrice,
                NumberOfParkingSpaces = a.NumberOfParkingSpaces,
                Status = a.Status,
                Type = a.Type,
            };
            return apartment;
        }


        //Get: Apartment
        [HttpGet]
        public async Task<IActionResult> ApartmentList()
        {
            List<Apartment> apartments = await _context.Apartments.ToListAsync();
            List<ApartmentViewModel> apartmentViewModels = new List<ApartmentViewModel>();

            foreach (Apartment apartment in apartments)
            {
                GetApartmentPhoto(apartment.Id);
                apartmentViewModels.Add(GetAllApartmentsFromModel(apartment));
            }

            return View(apartmentViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();
            foreach (ApplicationUser user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                var tenant = _context.TenantRentApartments
                                           .Where(t => t.TenantEmail == user.Email)
                                           .FirstOrDefault();

                //Check if this user is already a tenant
                if (tenant != null)
                    continue;
                allUsersViewModels.Add(GetViewModelFromModel(user));
            }
            return View(allUsersViewModels);
        }

        [HttpGet]
        public IActionResult GetAllTenants()
        {
            List<TenantRentApartment> tenants = _context.TenantRentApartments.ToList();
            List<TenantRentApartmentViewModel> allTenantsViewModels = new List<TenantRentApartmentViewModel>();
            foreach (TenantRentApartment tenant in tenants)
            {
                allTenantsViewModels.Add(GetTenantFromModel(tenant));
            }
            return View(allTenantsViewModels);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetAllUsersPartialViewAsync(long apartmentId)
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();

            foreach (ApplicationUser user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    continue;

                //Check if this apartment as been already assign to this user
                var output = _context.TenantRentApartments
                                     .Where(u => u.TenantEmail == user.Email &&  u.ApartmentId == apartmentId)
                                     .FirstOrDefault();
                if (output != null)
                    continue;

                allUsersViewModels.Add(GetViewModelFromModel(user));
            }
            var result = new BaseAllUsersViewModel();
            result.Users = allUsersViewModels;
            result.ApartmentId = apartmentId;
            return PartialView("_AssignTenantPartialView", result);
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


        public async Task SaveImageFile(IFormFile formFile)
        {
            if (formFile != null)
            {
                string folder = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles");
                string dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
                string fileName = $" ApartmentList {dateTime} {formFile.FileName}";

                using (FileStream fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                apartmentPhoto = new ApartmentPhoto()
                {
                    ImageURL = $"/Admin/{nameof(GetFile)}?filename={fileName}",
                    Part = part
                };
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
                string folder = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles");
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
                var contract = _context.FileModel
                                       .Where(f => f.Name == fileName)
                                       .FirstOrDefault();
                //contract can't be null
                contractId = contract.Id; 

            }
        }
        //Get: Apartment/AddApartment
        public IActionResult AddApartment()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddApartment(ApartmentViewModel apartmentViewModel)
        {
            if (ModelState.IsValid)
            {
                Apartment apartment = AddApartmenFromViewModel(apartmentViewModel);
                part = apartmentViewModel.Part;
                var fileModel = new FileModel();

                //look for the user who is currently Login
                ApplicationUser user = _context.Users
                                               .Where(u => u.UserName == User.Identity.Name)
                                               .First();

           
                    apartment.LessorId = user.Id;

                    await SaveImageFile(apartmentViewModel.ImageFile);

                    _context.Add(apartment);
                    await _context.SaveChangesAsync();

                    //Find the apartment id that we just add
                    var getApartmentId = _context.Apartments.Find(apartment.Id);

                    //Assign that apartment id to the apartmentId Foreign Key which is inside the ApartmentPhoto Table
                    if (getApartmentId != null)
                    {
                        apartmentPhoto.ApartmentId = getApartmentId.Id;

                        _context.Add(apartmentPhoto);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(ApartmentList));
            }
            return View(apartmentViewModel);
        }


        // Post: Admin/AddAsTenant/1
        [HttpGet]
        public IActionResult AddAsTenant(long tenantId, string email, long ChoosenApartmentId)
        {
            TenantRentApartmentViewModel result = new TenantRentApartmentViewModel
            {
                TenantId = tenantId,
                TenantEmail = email,
                ApartmentId = ChoosenApartmentId

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
                    var user = _userManager.Users
                                           .Where(u=> u.Id == model.TenantId)
                                           .FirstOrDefault();
                    
                    if (user != null)
                    {
                        TenantRentApartment tenantRentApartment;
                        var fileModel = new FileModel();
                   
                        await SaveFile(model.ContractFile, fileModel, user.Id);

                        tenantRentApartment = new TenantRentApartment()
                        {
                            TenantEmail = model.TenantEmail,
                            TenantPhoneNumber = user.PhoneNumber,
                            ApartmentId = model.ApartmentId,                             
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


                        //Change the apartment Status to Busy
                        var apartment = _context.Apartments
                                                .Where(a => a.Id == model.ApartmentId)
                                                .FirstOrDefault();
                        
                        if(apartment != null)
                        {
                            Apartment apartmt = new Apartment()
                            {
                                Id = apartment.Id,
                                Description = apartment.Description,
                                LessorId = apartment.LessorId,
                                LocatedAt = apartment.LocatedAt,
                                NumberOfRooms = apartment.NumberOfRooms,
                                NumberOfbathRooms = apartment.NumberOfbathRooms,
                                RoomArea = apartment.RoomArea,
                                FloorNumber = apartment.FloorNumber,
                                Price = apartment.Price,
                                DepositePrice = apartment.DepositePrice,
                                NumberOfParkingSpaces = apartment.NumberOfParkingSpaces,
                                Type = apartment.Type,
                                CreatedOn = apartment.CreatedOn,
                                Status = Data.Enum.ApartmentStatusEnum.Busy
                            };
                            _context.Update(apartmt);
                            await _context.SaveChangesAsync();
                        }

                        //Assign Tenant role to this user
                        await _userManager.AddToRoleAsync(user, "Tenant");

                        //Request to the database to find out the price on which the tenant and the lessor have agreed 
                        var apartmentPrice = _context.TenantRentApartments
                                                        .Where(t => t.TenantEmail == user.Email)
                                                        .Select(t => t.Price)
                                                        .FirstOrDefault();
                        decimal nbOfMonthPaid = 0;
                        PaymentHistory newPayment ;
                        TenantPaymentStatus tenantPaymentStatus;
                        RentPaymentDatesSchedular rentPaymentDatesSchedular;


                        //Sachant que le premier versement(1 ans de loyer) doit etre largement suprerieur au prix de l'apartement
                        if (model.AmountPaidByTenant > apartmentPrice)
                        {
                            nbOfMonthPaid = Decimal.Divide(model.AmountPaidByTenant, apartmentPrice);

                            /*Add 30 days on the current payment date in case the amount paid is not enough
                            Remind tenant after 30 days that he must pay his rent*/
                            newPayment = new PaymentHistory()
                            {
                                TenantEmail = user.Email,
                                AmountPaid = model.AmountPaidByTenant,
                                NunberOfMonthPaid = nbOfMonthPaid.ToString(),
                                PaidDate = DateTime.UtcNow
                            };
                            _context.Add(newPayment);
                            await _context.SaveChangesAsync();


                            //Schedule the next date to pay the rent 
                            rentPaymentDatesSchedular = new RentPaymentDatesSchedular
                            {
                                TenantEmail = user.Email,
                                IsRentPaidForThisDate = false,
                                AmmountSupposedToPay = apartmentPrice,
                                NextDateToPay = DateTimeOffset.UtcNow.AddMonths(Decimal.ToInt32(nbOfMonthPaid))
                            };
                            _context.Add(rentPaymentDatesSchedular);
                            await _context.SaveChangesAsync();


                            tenantPaymentStatus = new TenantPaymentStatus()
                            {
                                TenantEmail = user.Email,
                                NumberOfMonthsToPay = 0,
                                AmountRemainingForRent = 0,
                                RentStatus = Data.Enum.RentStatusEnum.Paid
                            };
                            _context.Add(tenantPaymentStatus);
                            await _context.SaveChangesAsync();
                        }//To Do: Should remove this one 
                        else if(model.AmountPaidByTenant < apartmentPrice)
                        {
                            newPayment = new PaymentHistory()
                            {
                                TenantEmail = user.Email,
                                AmountPaid = model.AmountPaidByTenant,
                                NunberOfMonthPaid = "The amount less than 1 month of payment",
                                PaidDate = DateTime.UtcNow
                            };
                            _context.Add(newPayment);
                            await _context.SaveChangesAsync();

                            //Schedule the next date to pay the rent 
                            rentPaymentDatesSchedular = new RentPaymentDatesSchedular
                            {
                                TenantEmail = user.Email,
                                IsRentPaidForThisDate = false,
                                NextDateToPay = DateTimeOffset.UtcNow.AddMonths(30)
                            };
                            _context.Add(rentPaymentDatesSchedular);
                            await _context.SaveChangesAsync();


                            tenantPaymentStatus = new TenantPaymentStatus()
                            {
                                TenantEmail = user.Email,
                                NumberOfMonthsToPay = 0,
                                AmountRemainingForRent = 0,
                                RentStatus = Data.Enum.RentStatusEnum.Partially_paid
                            };
                            _context.Add(tenantPaymentStatus);
                            await _context.SaveChangesAsync();
                        }
                                
                        return RedirectToAction(nameof(GetAllTenants));                       
                    }
                    else
                        return BadRequest($"The user {model.TenantEmail} does not even exist as simple user" );                   
                }
                return View(model);
            }
            catch(Exception ex)
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
