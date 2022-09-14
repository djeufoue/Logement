using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController( UserManager<ApplicationUser> userManager
            , ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
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

        public  IActionResult GetAllUsers()
        {
            List<ApplicationUser> users = _userManager.Users.ToList();
            List<AllUsersViewModel> allUsersViewModels = new List<AllUsersViewModel>();
            foreach(ApplicationUser user in users)
            {
                allUsersViewModels.Add(GetViewModelFromModel(user));
            }
            return View(allUsersViewModels);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
