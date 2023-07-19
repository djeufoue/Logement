using Logement.Data;
using Logement.Data.Enum;
using Logement.Models;
using Logement.Services;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.UserModel;
using PayPal.Api;
using System.Net;

namespace Logement.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        private readonly ILogger<CityController> _logger;
        private IHttpContextAccessor httpContextAccessor;

        public CityController(ApplicationDbContext context, ILogger<CityController> logger, IHttpContextAccessor ihcontext, IConfiguration configuration)
            : base(context, configuration)
        {
            _logger = logger;
            httpContextAccessor = ihcontext;
        }

        public async Task<JsonResult> CheckCityNameAvailability(string cityName)
        {
            var checkCityName = await dbc.Cities
                .Where(c => c.Name == cityName)
                .FirstOrDefaultAsync();

            if (checkCityName != null)
            {
                return Json(1);  //City name taken
            }
            else if (checkCityName == null)
                return Json(0); //City name not taken
            else
                return Json(-1);  //error occurred
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
                    Town = c.Town,
                    NumbersOfApartment = c.NumbersOfApartment,
                    Floor = c.Floor,
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
                    Town = c.Town,
                    NumbersOfApartment = c.NumbersOfApartment,
                    Floor = c.Floor,
                    DateAdded = c.DateAdded
                };
            }
            return city;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<CityViewModel> citiesModel = new List<CityViewModel>();
                var cities = await dbc.Cities.ToListAsync();

                if (cities != null)
                {
                    var CitiesMembers = await dbc.CityMembers
                            .Where(c => c.UserId == GetUser().Id && (c.Role == CityMemberRoleEnum.Landord || c.Role == CityMemberRoleEnum.Tenant))
                            .Include(c => c.City)
                            .Include(c => c.Apartment)
                            .ToListAsync();

                    foreach (var city in CitiesMembers)
                    {
                        if(city.Role == CityMemberRoleEnum.Landord)
                            citiesModel.Add(GetCitiesFromModel(city.City,0, null, city.Role));
                        else if( city.Role == CityMemberRoleEnum.Tenant)
                            citiesModel.Add(GetCitiesFromModel(city.City, city.Apartment.ApartmentNumber, null, city.Role));

                    }
                }
                return View(citiesModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public IActionResult AddCity()
        {
            CityViewModel city = new CityViewModel();
            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    City city = AddCityFromViewModel("AddCity", model);
                    var fileModel = new FileModel();

                    city.LandLordId = GetUser().Id;

                    dbc.Cities.Add(city);
                    await dbc.SaveChangesAsync();

                    string methodName = "AddCity";
                    await SaveImageFile(model.Image, city.Id, methodName);

                    CityMember cityMember = new CityMember
                    {
                        CityId = city.Id,
                        UserId = city.LandLordId,
                        Role = CityMemberRoleEnum.Landord
                    };
                    dbc.CityMembers.Add(cityMember);
                    await dbc.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return View(model);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckImageExistence(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await file.CopyToAsync(ms);
                        byte[] imageData = ms.ToArray();

                        var checkImage = await dbc.Fichiers
                            .Where(f => f.Data.SequenceEqual(imageData))
                            .FirstOrDefaultAsync();

                        if (checkImage != null)
                        {
                            return Json(1); // Image exists in the database
                        }
                        else
                        {
                            return Json(0); // Image doesn't exist in the database
                        }
                    }
                }

                return Json(-1); // No file or error occurred
            }
            catch (Exception)
            {
                return Json(-1); // Error occurred
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditCity(long id)
        {
            try
            {
                var city = await dbc.Cities.FindAsync(id);
                if (city == null)
                    return NotFound();

                var cityCreator = await GetCityCreator(id);
                if (cityCreator == null)
                    return Forbid();

                var cityIamage = await dbc.Fichiers.Where( img => img.CityId == id)
                    .FirstOrDefaultAsync();

                CityViewModel cityViewModel = GetCitiesFromModel(city,0, cityIamage, null);
                return View(cityViewModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditCity(long id, CityViewModel cityViewModel)
        {
            try
            {
                if (id != cityViewModel.Id)
                    return BadRequest();

                var city = await dbc.Cities.FindAsync(id);
                if (city == null)
                    return NotFound();

                var cityCreator = await GetCityCreator(id);
                if (cityCreator == null)
                    return Forbid();

                if (ModelState.IsValid)
                {
                    var dateAdded = await dbc.Cities
                        .Where(c => c.Id == id)
                        .Select(c => c.DateAdded)
                        .FirstOrDefaultAsync();

                    cityViewModel.DateAdded = dateAdded;

                    city = AddCityFromViewModel("EditCity", cityViewModel);
                    city.LandLordId = cityCreator.UserId;

                    dbc.Update(city);
                    await dbc.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                return View(cityViewModel);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpDelete, HttpGet]
        public async Task<IActionResult> DeleteCity(long cityId)
        {
            var city = await dbc.Cities.FindAsync(cityId);

            if (city == null)
                return NotFound();

           var  CityOwner = await GetCityCreator(cityId);

            if (CityOwner == null)
                return Forbid();

            var cityMembers = await dbc.CityMembers
                .Where( c => c.CityId == cityId)
                .ToListAsync();

            dbc.CityMembers.RemoveRange(cityMembers);
            await dbc.SaveChangesAsync();


            var cityImages = await dbc.Fichiers.Where(c => c.CityId == cityId).ToListAsync();

            dbc.Fichiers.RemoveRange(cityImages);
            await dbc.SaveChangesAsync();

            var cityApartments = await dbc.Apartments.Where( c => c.CityId == cityId).ToListAsync();

            if(cityApartments.Count > 0)
            {
                long apartmentId = 0;
                foreach(var apt in cityApartments)
                {
                    apartmentId = apt.Id;
                    break;
                }

                var tenantRentApartments = await dbc.TenantRentApartments
                    .Where(a => a.ApartmentId == apartmentId)
                    .ToListAsync();

                dbc.TenantRentApartments.RemoveRange(tenantRentApartments);
                await dbc.SaveChangesAsync();

                var contract = await dbc.FileModel
                    .Where(c => c.CityId == cityId)
                    .ToListAsync();

                if(contract.Count > 0)
                {
                    dbc.FileModel.RemoveRange(contract);
                    await dbc.SaveChangesAsync();
                }
                             
                foreach(var apt in cityApartments)
                {
                    //find all notifications that were send for a specific apartment inside a city
                    var notificationsSentForRentPayments = await dbc.NotificationSentForRentPayments
                       .Where(n => n.CityId == cityId && n.ApartmentNumber == apt.ApartmentNumber)
                       .ToListAsync();

                    if(notificationsSentForRentPayments.Count > 0)
                    {
                        dbc.NotificationSentForRentPayments.RemoveRange(notificationsSentForRentPayments);
                        await dbc.SaveChangesAsync();
                    }

                    var paymenHistories = await dbc.PaymentHistories
                      .Where(n => n.CityId == cityId && n.ApartmentNumber == apt.ApartmentNumber)
                      .ToListAsync();

                    if(paymenHistories.Count > 0)
                    {
                        dbc.PaymentHistories.RemoveRange(paymenHistories);
                        await dbc.SaveChangesAsync();
                    }
                }

                var apartmentImages = await dbc.Fichiers.Where(c => c.ApartmentId == apartmentId).ToListAsync();

                dbc.Fichiers.RemoveRange(apartmentImages);
                await dbc.SaveChangesAsync();

                dbc.Apartments.RemoveRange(cityApartments);
                await dbc.SaveChangesAsync();
            }

            var notificationSentForCitySubscription = await dbc.NotificationSentForSubscriptions
                .Where(n => n.CityId == cityId)
                .ToListAsync();

            if (notificationSentForCitySubscription.Count > 0)
            {
                dbc.NotificationSentForSubscriptions.RemoveRange(notificationSentForCitySubscription);
                await dbc.SaveChangesAsync();
            }

            var subscriptionPayment = await dbc.SubscriptionPayments
                .Where( s => s.CityId == cityId )
                .FirstOrDefaultAsync();

            if(subscriptionPayment != null )
            {
                dbc.SubscriptionPayments.Remove(subscriptionPayment);
                await dbc.SaveChangesAsync();
            }

            var rentPaymentDatesSchedulars = await dbc.RentPaymentDatesSchedulars
                .Where(r => r.CityId == cityId)
                .ToListAsync();

            if(rentPaymentDatesSchedulars.Count > 0)
            {
                dbc.RentPaymentDatesSchedulars.RemoveRange(rentPaymentDatesSchedulars);
                await dbc.SaveChangesAsync();
            }

            dbc.Cities.Remove(city);
            await dbc.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("EditCity/EditCityImage/{id}")]
        public async Task<ActionResult> EditCityImage(long id, IFormFile file)
        {
            try
            {
                var city = await dbc.Cities.FindAsync(id);
                if (city == null)
                    return NotFound();

                var cityCreator = await GetCityCreator(id);
                if (cityCreator == null)
                    return Forbid();

                var cityImage = await dbc.Fichiers.Where(img => img.CityId == id)
                    .FirstOrDefaultAsync();

                if(cityImage == null)
                    return NotFound("Image not found");

                if (file == null)
                    return RedirectToAction("EditCity", new { id = id });

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    var imageData = stream.ToArray();

                    cityImage.Data = imageData;
                    cityImage.ContentType = file.ContentType;
                    cityImage.FileName = file.FileName;
                    cityImage.UploadDate = DateTime.UtcNow;

                    dbc.Update(cityImage);
                    await dbc.SaveChangesAsync();

                    return RedirectToAction("EditCity", new { id = id });
                }
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task<ActionResult> PaymentWithPaypal(long cityId, string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
        {
            var ClientID = _configuration.GetValue<string>("PayPal:Key");
            var ClientSecret = _configuration.GetValue<string>("PayPal:Secret");
            var mode = _configuration.GetValue<string>("PayPal:mode");
            APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);

            try
            {
                string payerId = PayerID;
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = this.Request.Scheme + "://" + this.Request.Host + $"/City/PaymentWithPaypal?";
                    var guidd = Convert.ToString((new Random()).Next(10000));
                    guid = guidd;

                    var createPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);
                    var links = createPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    httpContextAccessor.HttpContext.Session.SetString("payment", createPayment.id);
                    httpContextAccessor.HttpContext.Session.SetInt32("cityId", Convert.ToInt32(cityId)); // Store the cityId in session
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("PaymentFailed");
                    }

                    var cityIdFromSession = httpContextAccessor.HttpContext.Session.GetInt32("cityId"); // Retrieve the cityId from session
                    if (cityIdFromSession.HasValue)
                    {
                        var citySubscription = await dbc.SubscriptionPayments
                            .Where(s => s.CityId == cityIdFromSession.Value)
                            .FirstOrDefaultAsync();

                        if (citySubscription == null)
                        {
                            SubscriptionPayment subscriptionPayment = new SubscriptionPayment()
                            {
                                PaymentId = paymentId,
                                CityId = Convert.ToInt64(cityIdFromSession.Value),
                                Amount = 5,
                                PaymentDate = DateTime.UtcNow,
                                NextPaymentDate = DateTimeOffset.UtcNow.AddMonths(1),
                                LandLordId = GetUser().Id,
                                IsPaid = true,
                            };
                            dbc.SubscriptionPayments.Add(subscriptionPayment);
                            dbc.SaveChanges();
                            return RedirectToAction("AddCity");
                        }
                        else
                        {
                            citySubscription.PaymentId = paymentId;
                            citySubscription.Amount += 5;
                            citySubscription.PaymentDate = DateTimeOffset.UtcNow;
                            citySubscription.NextPaymentDate = citySubscription.NextPaymentDate.AddMonths(1);
                        }
                    }
                    else
                    {
                        // Handle the scenario when cityId is not found in session
                        // Redirect or display an error message
                        return View("CityIdNotFound");
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return View(ex);
            }
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId,
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }


        private PayPal.Api.Payment payment;
        public Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
        {
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            itemList.items.Add(new Item()
            {
                name = "Item Detail",
                currency = "CAD",
                price = "0.01",
                quantity = "1",
                sku = "asd"
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var amount = new Amount()
            {
                currency = "CAD",
                total = "0.01",
            };
            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "DJE Residence subscription payment",
                invoice_number = Guid.NewGuid().ToString(),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "Sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            return this.payment.Create(apiContext);
        }

        public IActionResult PaymentFailed()
        {
            return View();
        }
    }
}
