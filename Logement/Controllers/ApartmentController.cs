using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    public class ApartmentController : Controller
    {
        public IActionResult Appartement()
        {
            return View();
        }
    }
}
