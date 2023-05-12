using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        protected async Task SaveImageFile(IFormFile file, long Id, string methodName)
        {
            if (file != null)
            {

                if (methodName == "AddApartment")
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        var imageData = stream.ToArray();

                        var image = new Fichier
                        {
                            Data = imageData,
                            ContentType = file.ContentType,
                            FileName = file.FileName,
                            CityOrApartement = "Apartement",
                            ApartmentId = Id,
                            UploadDate = DateTime.UtcNow
                        };
                        dbc.Fichiers.Add(image);
                        await dbc.SaveChangesAsync();
                    }
                }
                else if(methodName == "AddCity")
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        var imageData = stream.ToArray();

                        var image = new Fichier
                        {
                            Data = imageData,
                            ContentType = file.ContentType,
                            FileName = file.FileName,
                            CityOrApartement = "City",
                            CityId = Id,
                            UploadDate = DateTime.UtcNow
                        };
                        dbc.Fichiers.Add(image);
                        await dbc.SaveChangesAsync();
                    }
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
                    //NumberOfParkingSpaces = c.NumberOfParkingSpaces,
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
                    //NumberOfParkingSpaces = c.NumberOfParkingSpaces,
                    DateAdded = c.DateAdded
                };
            }
            return city;
        }

        protected CityViewModel GetCitiesFromModel(City city)
        {
            CityViewModel cityViewModel = new CityViewModel()
            {
                Id = city.Id,
                Name = city.Name,
                LocatedAt = city.LocatedAt,
                Floor = city.Floor,
                NumbersOfApartment = city.NumbersOfApartment,
                //NumberOfParkingSpaces = city.NumberOfParkingSpaces
            };
            return cityViewModel;
        }


        protected async Task<CityMember?> GetCityCreator(long cityId)
        {
            return await dbc.CityMembers
                .Include(c => c.User)
                .Where(c => c.CityId == cityId && c.Role == CityMemberRoleEnum.Landord && c.UserId == GetUser().Id)
                .FirstOrDefaultAsync();      
        }

        private ApplicationUser? _user;
        protected ApplicationUser GetUser()
        {
            if (_user == null && User.Identity != null)
                _user = dbc.Users
                     .Where(u => u.UserName == User.Identity.Name)
                     .First();
            return _user ?? new ApplicationUser();
        }
    }
}
