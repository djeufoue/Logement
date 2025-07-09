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
using System.Runtime.InteropServices;
using System.Security.Claims;

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

        protected long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

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
     
        protected CityViewModel GetCitiesFromModel(City city, long apartmentNumber, Fichier? cityImage = null, CityMemberRoleEnum? memberRole = null)
        {
            var subscription = dbc.SubscriptionPayments
                .Where(c => c.CityId == city.Id)
                .FirstOrDefault();

            CityViewModel cityViewModel = new CityViewModel()
            {
                Id = city.Id,
                Name = city.Name,
                ApartmentNumber = apartmentNumber, 
                CityMemberRole = memberRole,
                LocatedAt = city.LocatedAt,
                Town = city.Town,
                Floor = city.Floor,
                Data = cityImage != null ? cityImage.Data : null,
                ContentType = cityImage != null ? cityImage.ContentType : null,
                NumbersOfApartment = city.NumbersOfApartment,
                NextPaymentDate = subscription == null? DateTimeOffset.MinValue : subscription.NextPaymentDate,
            };
            return cityViewModel;
        }

        protected async Task<CityMember?> GetCityCreator(long cityId)
        {
            return await dbc.CityMembers
                .Include(c => c.User)
                .Where(c => c.CityId == cityId && c.Role == CityMemberRoleEnum.Landord && c.UserId == GetCurrentUser().Id)
                .FirstOrDefaultAsync();      
        }

        private ApplicationUser? _user;
        protected ApplicationUser GetCurrentUser()
        {
            if (_user == null && User.Identity != null)
                _user = dbc.Users
                     .Where(u => u.UserName == User.Identity.Name)
                     .First();
            return _user ?? new ApplicationUser();
        }
    }
}
