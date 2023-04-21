using Logement.Data;
using Logement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext dbc;
        public BaseController(ApplicationDbContext context)
        {
            dbc = context;
        }
        protected ApplicationUser GetUser()
        {
            return dbc.Users
                     .Where(u => u.UserName == User.Identity.Name)
                     .First();
        }
    }
}
