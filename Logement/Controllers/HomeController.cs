using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Diagnostics;
using System.Net;

namespace Logement.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration)
           : base(context, configuration)
        {
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<CityViewModel> citiesModel = new List<CityViewModel>();

                var cityImagesInfos = await dbc.Fichiers
                    .Include(img => img.City)
                    .Where( img => img.CityOrApartement == "City")
                    .ToListAsync();
                
                foreach (var cityImage in cityImagesInfos)
                {
                    citiesModel.Add(new CityViewModel
                    {
                        Id = cityImage.City.Id,
                        Name = cityImage.City.Name,
                        LocatedAt = cityImage.City.LocatedAt,
                        NumbersOfApartment = cityImage.City.NumbersOfApartment,
                        Floor = cityImage.City.Floor,
                        Data = cityImage.Data,
                        FileName = cityImage.FileName
                    });
                }
                return View(citiesModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}