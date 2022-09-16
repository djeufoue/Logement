using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net;

namespace Logement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        private string part;
        private long getTemplateId;
        private ApartmentPhoto apartmentPhoto = new ApartmentPhoto();
        private string GetPhoto;
        private string GetPart;

        public AdminController( UserManager<ApplicationUser> userManager
            , ApplicationDbContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
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

        /// <summary>
        ///Get the list of all apartments and pass them to the ApartmentViewModel before displaying them
        /// </summary>
        /// <param name="apartment"></param>
        /// <returns></returns>
        private ApartmentViewModel GetAllApartmentsFromModel(Apartment apartment)
        {
            GetApartmentPhoto(apartment.Id);
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

        /// <summary>
        /// This one will not work for now because the apartment table is empty
        /// </summary>
        /// <returns></returns>
        //Get: Apartment
        public async Task<IActionResult> ApartmentList()
        {
            List<Apartment> apartments = await _context.Apartments.ToListAsync();
            List<ApartmentViewModel> apartmentViewModels = new List<ApartmentViewModel>();

            foreach (Apartment apartment in apartments)
                apartmentViewModels.Add(GetAllApartmentsFromModel(apartment));

            return View(apartmentViewModels);
        }
        public IActionResult GetAllUsers()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();
            foreach (ApplicationUser user in users)
            {
                allUsersViewModels.Add(GetViewModelFromModel(user));
            }
            return View(allUsersViewModels);
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
                }
                if (GetPhoto != null && GetPhoto != null)
                    break;
                else
                    continue;
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

        public async Task SaveImageFile(IFormFile formFile, Apartment apartment)
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
                //apartment.ImageURL = $"/Apartment/{nameof(GetFile)}?filename={fileName}";

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

                await SaveImageFile(apartmentViewModel.ImageFile, apartment);
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
    }
}
