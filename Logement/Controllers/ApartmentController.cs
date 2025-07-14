using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Logement.Controllers
{
    [Authorize]
    public class ApartmentController : BaseController
    {
        UserManager<ApplicationUser> _userManager;
        public ApartmentController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
            : base(context, configuration)
        {
            _userManager = userManager;
        }

        [HttpPost]
        private async Task<IActionResult> AddApartment(ApartmentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await dbc.Users.Where( u => u.Id == UserId).FirstOrDefaultAsync();

                    if(currentUser != null)                    
                    {
                        var city = await dbc.Cities.Where(c => c.Id == model.CityId).FirstOrDefaultAsync();

                        if (city != null)
                        {
                            var cityCreator = GetCityCreator(city.Id);

                            if (cityCreator != null)
                            {
                                var apatment = await dbc.Apartments.
                                    Where(a => a.ApartmentName == model.ApartmentName)
                                    .FirstOrDefaultAsync();

                                if (apatment == null)
                                {
                                    apatment = new Apartment
                                    {
                                        ApartmentName = model.ApartmentName,
                                        CityId = city.Id,
                                        NumberOfRooms = model.NumberOfRooms,
                                        NumberOfbathRooms = model.NumberOfbathRooms,
                                        RoomArea = model.RoomArea,
                                        FloorNumber = model.FloorNumber,
                                        Price = model.Price,
                                        DepositePrice = model.DepositePrice,
                                        Status = model.Status,
                                        CreatedOn = DateTime.UtcNow,
                                        ApartmentAdderId = currentUser.Id,
                                    };

                                    await dbc.Apartments.AddAsync(apatment);
                                }
                                else
                                    return StatusCode((int)HttpStatusCode.Forbidden, "This apartment name already exists, choose another name");
                            }
                            else
                                return StatusCode((int)HttpStatusCode.Forbidden, "You do not have the necessary right to add an apartment to this city");
                        }
                        else
                            return StatusCode((int)HttpStatusCode.NotFound, "This city does not exist or was deleted");
                    }
                    else
                        return StatusCode((int)HttpStatusCode.NotFound,"This user does not exist or was deleted");
                }

                return Ok(model);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        } 

        private ApartmentViewModel GetAllApartmentsFromModel(Apartment apartment, City city, CityMember tenant)
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel()
            {
                Id = apartment.Id,
                LessorId = apartment.LessorId,
                TenantId = tenant.UserId,
                CityId = city.Id,
                ApartmentNunber = apartment.ApartmentNumber,
                CityName = city.Name,
                OccupiedBy = $"{tenant.User.FirstName} {tenant.User.LastName}",
                LocatedAt = city.LocatedAt,
                NumberOfRooms = apartment.NumberOfRooms,
                NumberOfbathRooms = apartment.NumberOfbathRooms,
                RoomArea = apartment.RoomArea,
                FloorNumber = apartment.FloorNumber,
                Price = apartment.Price,
                DepositePrice = apartment.DepositePrice,
                Status = apartment.Status,
                Type = apartment.Type,
            };
            return apartmentViewModel;
        }

        public async Task<JsonResult> CheckApartmentNumberAvailability(long apartmentNumber, long cityId)
        {
            var checkApartment = await dbc.Apartments
                .Where(a => a.CityId == cityId && a.ApartmentNumber == apartmentNumber)
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

        [HttpGet]
        public async Task<IActionResult> Index(long cityId)
        {
            try
            {
                var cityCreator = await dbc.CityMembers
                    .Where(c => c.UserId == GetCurrentUser().Id && c.CityId == cityId && c.Role == CityMemberRoleEnum.Landord)
                    .FirstOrDefaultAsync();

                if (cityCreator != null)
                {
                    List<ApartmentViewModel> apartmentViewModel = new List<ApartmentViewModel>();

                    var apartmentList = await dbc.Apartments
                        .Where(a => a.CityId == cityId)
                        .Include(a => a.City)
                        .ToListAsync();

                    foreach (Apartment apartment in apartmentList)
                    {
                        var tenantInside = dbc.CityMembers
                            .Where(cm => cm.CityId == cityId && cm.ApartmentId == apartment.Id)
                            .Include(cm => cm.User)
                            .FirstOrDefault();

                        if (tenantInside == null)
                            continue;

                        var city = await dbc.Cities
                            .Where(c => c.Id == cityId)
                            .FirstOrDefaultAsync();

                        apartmentViewModel.Add(GetAllApartmentsFromModel(apartment, city, tenantInside));
                    }

                    ViewData["cityId"] = cityId;
                    return View(apartmentViewModel);
                }
                else
                    return Forbid();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task<IActionResult> ApartmentDetails(long? cityId, long? apartmentId)
        {
            if (cityId == null || apartmentId == null)
            {
                return NotFound();
            }

            var apartmentsInfos = await dbc.Apartments
                         .Include(a => a.City)
                         .Include(a => a.Lessor)
                         .Where(a => a.CityId == cityId && a.Id == apartmentId)
                         .Select(a => new
                         {
                             Id = a.Id,
                             LessorId = a.LessorId,
                             PhoneNumber = a.Lessor.PhoneNumber,
                             Email = a.Lessor.Email,
                             Price = a.Price,
                             NumberOfRooms = a.NumberOfRooms,
                             RoomArea = a.RoomArea,
                             NumberOfbathRooms = a.NumberOfbathRooms,
                             FloorNumber = a.FloorNumber,
                             Type = a.Type,
                             LocatedAt = a.City.LocatedAt,
                             CityName = a.City.Name,
                             PropertyId = a.City.Id,
                         }).FirstOrDefaultAsync();

            if (apartmentsInfos == null)
                return NotFound();

            var tenant = await dbc.TenantRentApartments
                .Where(t => t.ApartmentId == apartmentsInfos.Id && t.TenantId == GetCurrentUser().Id)
                .FirstOrDefaultAsync();


            if (tenant == null && apartmentsInfos.LessorId != GetCurrentUser().Id)
                return Forbid("This apartment does not belong to you!");

            //Tenant or landlord
            else if (tenant != null || apartmentsInfos.LessorId == GetCurrentUser().Id)
            {
                ApartmentBaseInfos apartmentBaseInfos = new ApartmentBaseInfos();

                apartmentBaseInfos.ApartmentInfos = new ApartmentDescriptions
                {
                    Id = apartmentsInfos.Id,
                    CityName = apartmentsInfos.CityName,
                    PropertyId = apartmentsInfos.PropertyId,
                    Price = (Int32)apartmentsInfos.Price,
                    NumberOfRooms = apartmentsInfos.NumberOfRooms,
                    NumberOfbathRooms = apartmentsInfos.NumberOfbathRooms,
                    RoomArea = apartmentsInfos.RoomArea,
                    FloorNumber = (Int32)apartmentsInfos.FloorNumber,
                    ApartmentType = apartmentsInfos.Type,
                    LocatedAt = apartmentsInfos.LocatedAt,
                    LandlordId = apartmentsInfos.LessorId,
                    LandlordPhoneNumber = apartmentsInfos.PhoneNumber,
                    LandlordEmail = apartmentsInfos.Email,
                };

                var apartmentImages = await dbc.Fichiers
                    .Where(a => a.CityOrApartement == "Apartement" && a.ApartmentId == apartmentsInfos.Id)
                    .ToListAsync();

                if (apartmentImages.Count == 0)
                    return NotFound("There is no image for this apartment, please contact the system administrator");

                foreach (var image in apartmentImages)
                {
                    apartmentBaseInfos.ApartmentImages.Add(new ApartmentImagesInfos
                    {
                        Data = image.Data,
                        ContentType = image.ContentType
                    });
                }
                return View(apartmentBaseInfos);
            }
            else
                return Forbid();
        }

        private AllUsersViewModel GetViewModelFromModel(long cityId, ApplicationUser user)
        {
            AllUsersViewModel allUsersViewModel = new AllUsersViewModel()
            {
                Id = user.Id,
                CityId = cityId,
                TenantFullName = $"{user.FirstName} {user.LastName}",
                TenantId = user.Id,
                PhoneNumber = user.PhoneNumber
            };
            return allUsersViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(long cityId)
        {
            try
            {
                List<ApplicationUser> users = _userManager.Users.ToList();
                List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();

                var landlord = await GetCityCreator(cityId);
                if (landlord == null)
                    return Forbid();

                foreach (ApplicationUser user in users)
                {
                    //We can assign different apartments to the same user within the same City
                    var cityMember = await dbc.CityMembers
                        .Where(cm => cm.UserId == user.Id && cm.CityId == cityId)
                        .FirstOrDefaultAsync();

                    // Check if current user is the systemAdmin 
                    var isSystemAdmin = await _userManager.IsInRoleAsync(user, "SystemAdmin");


                    //If it is the landlord of this city
                    if (cityMember != null && cityMember.UserId == landlord.UserId)
                        continue;
                    else if (isSystemAdmin)
                        continue;
                    else
                        allUsersViewModels.Add(GetViewModelFromModel(cityId, user));

                }
                ViewData["cityId"] = cityId;
                return View(allUsersViewModels);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
