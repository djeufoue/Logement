using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    public class TenantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
