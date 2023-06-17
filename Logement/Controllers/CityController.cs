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
        
        public CityController(ApplicationDbContext context,ILogger<CityController> logger, IHttpContextAccessor ihcontext, IConfiguration configuration)
            : base(context, configuration)
        {
            _logger = logger;
            httpContextAccessor = ihcontext;
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
                    var creatorCities = await dbc.CityMembers
                            .Where(c => c.UserId == GetUser().Id && c.Role == CityMemberRoleEnum.Landord)
                            .Include(c => c.City)
                            .ToListAsync();

                    foreach (var city in creatorCities)
                        citiesModel.Add(GetCitiesFromModel(city.City));
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

                    foreach (var file in model.Image)
                    {
                        string methodName = "AddCity";
                        await SaveImageFile(file, city.Id, methodName);
                    }

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

                CityViewModel cityViewModel = GetCitiesFromModel(city);
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
        public Payment CreatePayment (APIContext apiContext, string redirectUrl, string blogId)
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
