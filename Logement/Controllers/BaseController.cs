using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net;

namespace Logement.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext dbc;
        protected readonly IConfiguration _configuration;

        public BaseController(ApplicationDbContext context, IConfiguration configuration)
        {
            dbc = context;
            _configuration = configuration;
        }

        protected async Task SaveImageFile(IFormFile formFile, long Id, string? photoPart, string methodName)
        {
            if (formFile != null)
            {
                string? folder = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles");
                string dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss");
                string fileName = $" ApartmentList {dateTime} {formFile.FileName}";

                using (FileStream fileStream = new FileStream(Path.Combine(folder, fileName), FileMode.Create, FileAccess.Write))
                {
                    await formFile.CopyToAsync(fileStream);
                }

                if (methodName == "AddApartment")
                {
                    ApartmentPhoto apartmentPhoto = new ApartmentPhoto()
                    {
                        ApartmentId = Id,
                        ImageURL = $"/Admin/{nameof(GetFile)}?filename={fileName}",
                        Part = photoPart
                    };
                    dbc.ApartmentPhotos.Add(apartmentPhoto);
                    await dbc.SaveChangesAsync();
                }
                else if (methodName == "AddCity")
                {
                    CityPhoto cityPhoto = new CityPhoto()
                    {
                        CityId = Id,
                        Size = formFile.Length,
                        ImageURL = $"/Admin/{nameof(GetFile)}?filename={fileName}"
                    };
                    dbc.CityPhotos.Add(cityPhoto);
                    await dbc.SaveChangesAsync();
                }
            }
        }

        [AllowAnonymous]
        protected IActionResult GetFile(string fileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    string filePathName = _configuration.GetValue<string>("PathToApartmentsImagesAndFiles") + fileName;

                    if (System.IO.File.Exists(filePathName))
                    {
                        var modificationDate = System.IO.File.GetLastWriteTimeUtc(filePathName);
                        FileStream fileStream = System.IO.File.OpenRead(filePathName);
                        string contentType = MimeTypes.GetMimeType(fileName);

                        var contenDisposotion = new System.Net.Mime.ContentDisposition()
                        {
                            FileName = fileName,
                            Inline = true,
                            Size = fileStream.Length,
                            ModificationDate = modificationDate
                        };
                        Response.Headers.Add("Content-Disposition", contenDisposotion.ToString());

                        return File(fileStream, contentType);
                    }
                    else return NotFound($"File '{fileName}' not found");
                }
                else return BadRequest("File name not Provided");
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private City AddCityFromViewModel(string method, CityViewModel c)
        {
            City city = new City();

            if (method == "AddCity")
            {
                city = new City()
                {
                    Id = c.Id,
                    Name = c.Name,
                    LocatedAt = c.LocatedAt,
                    NumbersOfApartment = c.NumbersOfApartment,
                    Floor = c.Floor,
                    NumberOfParkingSpaces = c.NumberOfParkingSpaces,
                    DateAdded = DateTime.UtcNow
                };
            }
            else if (method == "EditCity")
            {
                city = new City()
                {
                    Id = c.Id,
                    Name = c.Name,
                    LocatedAt = c.LocatedAt,
                    NumbersOfApartment = c.NumbersOfApartment,
                    Floor = c.Floor,
                    NumberOfParkingSpaces = c.NumberOfParkingSpaces,
                    DateAdded = c.DateAdded
                };
            }
            return city;
        }

        protected CityViewModel GetCitiesFromModel(City city, string? cityImage)
        {
            CityViewModel cityViewModel = new CityViewModel()
            {
                Id = city.Id,
                Name = city.Name,
                LocatedAt = city.LocatedAt,
                CityPhoto = cityImage,
                Floor = city.Floor,
                NumbersOfApartment = city.NumbersOfApartment,
                NumberOfParkingSpaces = city.NumberOfParkingSpaces
            };
            return cityViewModel;
        }


        protected async Task<CityMember?> GetCityCreator(long cityId)
        {
            return await dbc.CityMembers
                         .Where(c => c.CityId == cityId && c.Role == CityMemberRoleEnum.Admin && c.UserId == GetUser().Id)
                         .FirstOrDefaultAsync();      
        }

        protected ApplicationUser GetUser()
        {
            return dbc.Users
                     .Where(u => u.UserName == User.Identity.Name)
                     .First();
        }
    }
}
