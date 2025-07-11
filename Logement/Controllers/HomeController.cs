﻿using Logement.Data;
using Logement.Models;
using Logement.Schedular;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;

namespace Logement.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private BaseScheduler baseScheduler;

        public HomeController(ILogger<HomeController> logger,
            Services.EmailService emailService,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            IConfiguration configuration, Services.SMSservice smsService)
           : base(context, configuration)
        {
            _logger = logger;
            baseScheduler = new BaseScheduler(context, logger, emailService, userManager, smsService);
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
                        Price = apartment.Price,
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
                var apartment = await dbc.Apartments
                        .Include(a => a.City)
                        .Include(a => a.Lessor)
                        .Select(a => new
                        {
                            Id = a.Id,
                            LessorId = a.LessorId,
                            PropertyId = a.City.Id,
                            PhoneNumber = a.Lessor.PhoneNumber,
                            Email = a.Lessor.Email,
                            Price = a.Price,
                            NumberOfRooms = a.NumberOfRooms,
                            RoomArea = a.RoomArea,
                            NumberOfbathRooms = a.NumberOfbathRooms,
                            FloorNumber = a.FloorNumber,
                            Type = a.Type,
                            LocatedAt = a.City.LocatedAt
                        })
                        .Where(a => a.Id ==  apartmentId)
                        .FirstOrDefaultAsync();

                if (apartment == null)
                    return NotFound("This apartment was not found or was deleted");

                ApartmentBaseInfos apartmentBaseInfos = new ApartmentBaseInfos();

                apartmentBaseInfos.ApartmentInfos = new ApartmentDescriptions
                {
                    Id = apartment.Id,
                    Price = (Int32)apartment.Price,
                    NumberOfRooms = apartment.NumberOfRooms,
                    NumberOfbathRooms = apartment.NumberOfbathRooms,
                    RoomArea = apartment.RoomArea,
                    FloorNumber = (Int32)apartment.FloorNumber,
                    ApartmentType = apartment.Type,
                    LocatedAt = apartment.LocatedAt,
                    LandlordId = apartment.LessorId,
                    LandlordPhoneNumber = apartment.PhoneNumber,
                    LandlordEmail = apartment.Email,
                    PropertyId = apartment.PropertyId,
                };

                var apartmentImages = await dbc.Fichiers
                    .Where(a => a.CityOrApartement == "Apartement" && a.ApartmentId == apartmentId)
                    .ToListAsync();

                if (apartmentImages.Count == 0)
                    return NotFound("There is no image for this apartment, please contact the system administrator");

                foreach (var image in apartmentImages)
                {
                    apartmentBaseInfos.ApartmentImages.Add(new ApartmentImagesInfos
                    {
                        Data = image.Data,
                        ContentType = image.ContentType
                    });
                }

                var apartmentTenancies = await dbc.Tenancies
                    .Where(at => at.ApartmentId == apartment.Id)
                    .ToListAsync();

                if(apartmentTenancies != null && apartmentTenancies.Count > 0)
                {
                    apartmentBaseInfos.ApartmentTenancies = new List<DTO.TenancyDTO>();
                    foreach (var tenancy in apartmentTenancies)
                    {
                        apartmentBaseInfos.ApartmentTenancies.Add(new DTO.TenancyDTO
                        {
                            LeaseStartDate = tenancy.LeaseStartDate,
                            LeaseExpiryDate = tenancy.LeaseExpiryDate,
                            Status = tenancy.Status.ToString(),
                            TenancyId = tenancy.Id,
                        });
                    }
                }

                return View(apartmentBaseInfos);
            }
            catch(Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SendMessage(long apartmentId, string name, string email, string message, string landlordEmail)
        {
            string emailSubject = "New Message from Apartment Contact Form";
            string emailBody = $"<p>Name: {name}\n</p> <p>Email: {email}\n</p> <p>Message:\n{message}</p>";

            await baseScheduler.SendEmail(landlordEmail, emailSubject, emailBody);
            return RedirectToAction("GetAllApartmentImages", new { apartmentId = apartmentId });
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