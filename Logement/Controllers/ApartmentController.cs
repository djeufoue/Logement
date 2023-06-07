using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Logement.Controllers
{
    public class ApartmentController : BaseController
    {
        UserManager<ApplicationUser> _userManager;
        public ApartmentController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
            :base(context, configuration)
        {
            _userManager = userManager;
        }

        private ApartmentViewModel GetAllApartmentsFromModel(Apartment apartment, City city, CityMember tenant)
        {
            ApartmentViewModel apartmentViewModel = new ApartmentViewModel()
            {
                Id = apartment.Id,
                LessorId = apartment.LessorId,
                ApartmentNunber = apartment.ApartmentNumber,
                CityName = city.Name,
                OccupiedBy = $"{tenant.User.FirstName} {tenant.User.LastName}",
                Description = apartment.Description,
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

        public async Task<JsonResult> CheckApartmentNumberAvailability(long apartmentNumber)
        {
            var checkApartment = await dbc.Apartments
                .Where(a => a.ApartmentNumber == apartmentNumber)
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
                    .Where(c => c.UserId == GetUser().Id && c.CityId == cityId && c.Role == CityMemberRoleEnum.Landord)
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

        private AllUsersViewModel GetViewModelFromModel(long cityId, ApplicationUser user)
        {
            AllUsersViewModel allUsersViewModel = new AllUsersViewModel()
            {
                Id = user.Id,
                CityId = cityId,
                TenantFullName = $"{user.FirstName} {user.LastName}",
                TenantId = user.Id,
                JobTitle = user.JobTitle,
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
                        .Where(cm => cm.UserId == user.Id && cm.CityId == cityId 
                         && cm.Role == CityMemberRoleEnum.Landord)
                        .FirstOrDefaultAsync();

                    if (cityMember == null)
                    {
                        allUsersViewModels.Add(GetViewModelFromModel(cityId, user));
                    }
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
