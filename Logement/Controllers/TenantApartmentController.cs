using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    [Authorize(Roles ="Tenant")]
    public class TenantApartmentController : Controller
    {
        public IActionResult MyRentInfo()
        {
            return View();
        }
    }
}
