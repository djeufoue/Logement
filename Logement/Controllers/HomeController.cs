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
                CityHomePageViewModel citiesModel = new CityHomePageViewModel();

                var cityImagesInfos = await dbc.Fichiers
                    .Include(img => img.City)
                    .Where( img => img.CityOrApartement == "City")
                    .ToListAsync();

                string? firsCityImage = cityImagesInfos.Select(i => $"data:{i.ContentType};base64,{Convert.ToBase64String(i.Data)}").FirstOrDefault(); 

                foreach (var cityImage in cityImagesInfos)
                {
                    citiesModel.cityViewModel.Add(new CityViewModel
                    {
                        Id = cityImage.City.Id,
                        Name = cityImage.City.Name,
                        LocatedAt = cityImage.City.LocatedAt,
                        Town = cityImage.City.Town,
                        NumbersOfApartment = cityImage.City.NumbersOfApartment,
                        Floor = cityImage.City.Floor,
                        Data = cityImage.Data,
                        ContentType = cityImage.ContentType,
                        FileName = cityImage.FileName
                    });
                }
                citiesModel.FirstImage = firsCityImage;
                return View(citiesModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetImages()
        {
            try
            {
                var cityImages = await dbc.Fichiers
                   .Include(f => f.City)
                   .Where(f => f.CityOrApartement == "City")
                   .Select(f => new
                   {
                       ContentType = f.ContentType,
                       Data = Convert.ToBase64String(f.Data),
                       FileName = f.FileName
                   }).ToListAsync();

                return Ok(cityImages);
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