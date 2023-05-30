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
                    citiesModel.CityViewModel.Add(new CityViewModel
                    {
                        Id = cityImage.City.Id,
                        Name = cityImage.City.Name,
                        LocatedAt = cityImage.City.LocatedAt,
                        Town = cityImage.City.Town,
                        NumbersOfApartment = cityImage.City.NumbersOfApartment,
                        Floor = cityImage.City.Floor,
                        Data = cityImage.Data,
                        ContentType = cityImage.ContentType
                    });
                }
                citiesModel.FirstImage = firsCityImage;

                var apartmentsInfos = await dbc.Apartments
                    .Include(a => a.City)
                    .Select(a => new
                    {
                      Id = a.Id,
                      Price = a.Price,
                      LocatedAt = a.City.LocatedAt
                    }).ToListAsync();

                foreach(var apartment in apartmentsInfos)
                {
                    //Select the first apartment image we found for this apartment
                    var apartmentImage = await dbc.Fichiers
                        .Where(a => a.ApartmentId == apartment.Id && a.CityOrApartement == "Apartement")
                        .FirstOrDefaultAsync();

                    if(apartmentImage == null)
                    {
                        _logger.LogError($"The apartment with the id {apartment.Id} does not have any images");
                        continue;
                    }
                        
                    citiesModel.Apartment.Add(new ApartmentInfos
                    {
                        Id = apartment.Id,
                        Price = (Int32)apartment.Price,
                        LocatedAt = apartment.LocatedAt,
                        Data = apartmentImage.Data,
                        ContentType = apartmentImage.ContentType
                    });
                }
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllApartmentImages(long apartmentId)
        {          
            try
            {
                var apartmemt = await dbc.Apartments
               .Where(a => a.Id == apartmentId)
               .FirstOrDefaultAsync();

                if (apartmemt == null)
                    return NotFound();

                var apartmentsInfos = await dbc.Apartments
                        .Include(a => a.City)
                        .Include(a => a.Lessor)
                        .Select(a => new
                        {
                            Id = a.Id,
                            LessorId = a.LessorId,
                            PhoneNumber = a.Lessor.PhoneNumber,
                            Email = a.Lessor.Email,
                            Price = a.Price,
                            NumberOfRooms = a.NumberOfRooms,
                            RoomArea = a.RoomArea,
                            FloorNumber = a.FloorNumber,
                            Type = a.Type,
                            Description = a.Description,
                            LocatedAt = a.City.LocatedAt
                        }).FirstOrDefaultAsync();

                ApartmentBaseInfos apartmentBaseInfos = new ApartmentBaseInfos();

                apartmentBaseInfos.apartmentInfos = new ApartmentDescriptions
                {
                    Id = apartmentsInfos.Id,
                    Price = (Int32)apartmentsInfos.Price,
                    NumberOfRooms = apartmentsInfos.NumberOfRooms,
                    RoomArea = apartmentsInfos.RoomArea,
                    FloorNumber = (Int32)apartmentsInfos.FloorNumber,
                    ApartmentType = apartmentsInfos.Type,
                    Description = apartmentsInfos.Description,
                    LocatedAt = apartmentsInfos.LocatedAt,
                    LandlordId = apartmentsInfos.LessorId,
                    LandlordPhoneNumber = apartmentsInfos.PhoneNumber,
                    LandlordEmail = apartmentsInfos.Email,
                };

                var apartmentImages = await dbc.Fichiers
                    .Where(a => a.CityOrApartement == "Apartement" && a.ApartmentId == apartmentId)
                    .ToListAsync();

                if (apartmentImages.Count == 0)
                    return NotFound("There is no image for this apartment, please contact the system administrator");

                foreach (var image in apartmentImages)
                {
                    apartmentBaseInfos.apartmentImages.Add(new ApartmentImagesInfos
                    {
                        Data = image.Data,
                        ContentType = image.ContentType
                    });
                }
                return Ok(apartmentBaseInfos);
            }
            catch(Exception e)
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