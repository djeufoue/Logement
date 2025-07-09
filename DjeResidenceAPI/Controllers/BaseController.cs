using DjeResidenceAPI.Data;
using DjeResidenceAPI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DjeResidenceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext dbc;
        protected readonly IConfiguration _configuration;

        public BaseController(ApplicationDbContext context, IConfiguration configuration)
        {
            dbc = context;
            _configuration = configuration;
        }

        protected long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        private ApplicationUser _user;

        protected ApplicationUser GetUser()
        {
            if (_user == null && User.Identity != null)
                _user = dbc.Users
                     .Where(u => u.UserName == User.Identity.Name)
                     .First();
            return _user ?? new ApplicationUser();
        }
    }
}
