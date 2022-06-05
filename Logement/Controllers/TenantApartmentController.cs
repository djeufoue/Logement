using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    public class TenantApartmentController : Controller
    {
        public IActionResult MyRentInfo()
        {
            return View();
        }
    }
}
