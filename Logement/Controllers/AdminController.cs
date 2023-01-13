using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Crypto.Tls;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Logement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TenantRentApartmentViewModel _tenantRentApartmentViewModel;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        protected readonly ILogger<T> _logger;

        private string part;
        private long getTemplateId;
        private ApartmentPhoto apartmentPhoto = new ApartmentPhoto();
        TenantRentApartment tenantRentApartment = new TenantRentApartment();
        private string  GetPhoto;
        private string GetPart;


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
                TenantId = tenant.TenantId,
                BailId = tenant.BailId,
                TenantEmail = tenant.Tenant.Email,
                Price = tenant.Price,
                DepositePrice = tenant.DepositePrice,
                PaymentMethod = tenant.PaymentMethodEnum,
                NumberOfMonthsToPay = tenant.NumberOfMonthsToPay,
                AmountRemainingForRent = tenant.AmountRemainingForRent,
                StartOfContract = tenant.StartOfContract.UtcDateTime,
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
                PaymentMethod = apartment.paymentMethod,
                TemplateContractId = apartment.TemplateContractId,
                NumberOfParkingSpaces = apartment.NumberOfParkingSpaces,
                Status = apartment.Status,
                Type = apartment.Type,
                ImageURL = GetPhoto,
                Part = GetPart,
                CreatedOn = apartment.CreatedOn.UtcDateTime
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
                paymentMethod = a.PaymentMethod,
                TemplateContractId = a.TemplateContractId,
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
                allUsersViewModels.Add(GetViewModelFromModel(user));
            }
            return View(allUsersViewModels);
        }

        [HttpGet]
        public IActionResult GetAllTenants()
        {
            List<TenantRentApartment> tenants = _context.TenantRentApartments.ToList();
            List<TenantRentApartmentViewModel> allTenantsViewModels = new List<TenantRentApartmentViewModel>();
            /*foreach( TenantRentApartment tenant in tenants)
            {
                allTenantsViewModels.Add(GetTenantFromModel(tenant));
            }*/
            return View(allTenantsViewModels);
        }

        
        public IActionResult GetAllUsersPartialView()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();
            foreach (ApplicationUser user in users)
            {
                allUsersViewModels.Add(GetViewModelFromModel(user));
            }
            return PartialView("_AssignTenantPartialView", allUsersViewModels);
        }

        public IActionResult AllAccess()
        {
            return View();
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
        public async Task SaveFile(IFormFile formFile, FileModel fileModel)
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
                        Name = fileName,
                        Size = fileStream.Length
                    };
                    await formFile.CopyToAsync(fileStream);
                }
                fileModel.FileURL = $"/Admin/{nameof(GetFile)}?filename={fileName}";

                _context.Add(fileModel);
                await _context.SaveChangesAsync();

                var file = _context.FileModel.Find(fileModel.Id);

                if (file != null)
                    getTemplateId = file.Id;
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
                    await SaveFile(apartmentViewModel.UploadTemplateContract, fileModel);

                    apartment.TemplateContractId = getTemplateId;

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
        public IActionResult AddAsTenant(long? Id)
        {         
           return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTenantRentInfos(TenantRentApartmentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var GivenUser = new TenantRentApartment();
                    var user = await _userManager.FindByEmailAsync(model.TenantEmail);
                    
                    if (user != null)
                    {
                        GivenUser = _context.TenantRentApartments
                                            .Where(u => u.TenantId == user.Id)
                                            .FirstOrDefault();
                    }                  
              
                    TenantRentApartment tenantRentApartment = new TenantRentApartment();
                    var fileModel = new FileModel();

                    if (model.AmountPaidByTenant > model.Price)
                    {
                        model.AmountPaidInAdvance = model.AmountPaidByTenant - model.Price;
                        model.NumberOfMonthsToPay = 0;
                        model.AmountRemainingForRent = 0;                                     
                    }

                    //False if the email was not yet registered as a tenant 
                    if (GivenUser == null)
                    {
                        if (user != null)
                        {
                            await SaveFile(model.ContractFile, fileModel);

                            tenantRentApartment = new TenantRentApartment()
                            {
                                TenantId = user.Id,
                                BailId = getTemplateId,
                                Price = model.Price,
                                AmountPaidByTenant = model.AmountPaidByTenant,
                                AmountPaidInAdvance = model.AmountPaidInAdvance,
                                DepositePrice = model.DepositePrice,
                                AmountRemainingForRent = model.AmountRemainingForRent,
                                NumberOfMonthsToPay = model.NumberOfMonthsToPay,
                                PaymentMethodEnum = model.PaymentMethod,
                                StartOfContract = model.StartOfContract,
                                EndOfContract = model.EndOfContract
                            };

                            _context.Add(tenantRentApartment);
                            _userManager.AddToRoleAsync(user, "Tenant").Wait();
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(GetAllTenants));
                        }
                        else
                            return NotFound($"The user {model.TenantEmail} doesn't exist");
                    }
                    else
                        return BadRequest($"The user {model.TenantEmail} already exists as tenant");
                }
                return View(model);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }           
        }

        [HttpGet]
        public IActionResult ChooseApartment(long? Id)
        {
            try
            {
                if (Id == null)
                    return NotFound();

                Apartment? apartment = _context.Apartments.Find(Id);

                if (apartment == null)
                    return NotFound();

                ApplicationUser user = _context.Users
                                               .Where(u => u.UserName == User.Identity.Name)
                                               .First();

                if (apartment.LessorId != user.Id)
                    return Forbid("Seul le bailleur a le droit de céder un logement à un locataire.");

                return View();

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        //To do: need to change some propertis type from non nullable to nullable(TenantRentApartment Table)
        [HttpPut]
        public async Task<IActionResult> ChooseApartment(long id, string tenantEmail)
        {
            try
            {

                if (!string.IsNullOrEmpty(tenantEmail))
                {
                    List<TenantRentApartment> tenantinfos = await _context.TenantRentApartments.ToListAsync();
                    TenantRentApartment newTenant; 

                    foreach (var tenant in tenantinfos)
                    {
                        //It means that this aparment has been already assign to a tenant
                        if (tenant.ApartmentId == id)
                            return View();
                    }

                    var tenantRentApartment = _context.TenantRentApartments.Find(tenantEmail)?.TenantId;

                    if (tenantRentApartment != null)
                    {
                        newTenant = new TenantRentApartment()
                        {
                            ApartmentId = id,
                        };

                        _context.Add(newTenant);
                      // await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(AllMyTenant)); 
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        protected ActionResult InternalServerError(Exception ex, string message = null)
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
