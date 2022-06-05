using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
