using Logement.Data;
using Logement.Models;
using Logement.Services;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using Twilio.Types;

namespace Logement.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly EmailService _emailService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
             EmailService emailService,
            ApplicationDbContext context,
             IConfiguration configuration,
            ILogger<AccountController> logger) : base(context, configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        [AllowAnonymous]
        public async Task<JsonResult> CheckPhoneNumberAvailability(string countryCode ,string phoneNumber)
        {
            var checkPhone = await dbc.Users
                .Where(p => p.PhoneNumber == phoneNumber && p.CountryCode == countryCode)
                .FirstOrDefaultAsync();

            if (checkPhone != null)
            {
                return Json(1);  //phone number taken
            }
            else if (checkPhone == null)
                return Json(0); //phone number not taken
            else
                return Json(-1);  //error occurred
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
                return View(registerViewModel);

            string email = registerViewModel.Email;
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                ModelState.AddModelError(nameof(email), $"User account {email} already exists");
                return View(registerViewModel);
            }

            if (!String.IsNullOrEmpty(registerViewModel.PhoneNumber))
            {
                string? phoneNumber = registerViewModel.PhoneNumber;
                var checkPhone = await dbc.Users
                    .Where(u => u.PhoneNumber == registerViewModel.PhoneNumber && u.CountryCode == registerViewModel.CountryCode)
                    .FirstOrDefaultAsync();

                if (checkPhone != null)
                {
                    ModelState.AddModelError(nameof(phoneNumber), $"User account {registerViewModel.CountryCode} {phoneNumber} already exists");
                    return View(registerViewModel);
                }
            }

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = registerViewModel.PhoneNumber,
                FirstName = registerViewModel.FirstName,
                CountryCode = registerViewModel.CountryCode,
                LastName = registerViewModel.LastName
            };
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);
            if (result.Succeeded)
            {
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    throw new ApplicationException("SendConfirmationEmail is not implemented");
                }
                _logger.LogInformation($"User account {email} created successfully.");

                return RedirectToAction(nameof(Login));
            }
            else
            {
                var errorsStr = JsonConvert.SerializeObject(result.Errors);
                var modelStr = JsonConvert.SerializeObject(registerViewModel);
                _logger.LogWarning($"Failed to Register. userDTO: {modelStr} errors:{errorsStr}");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(registerViewModel.Password), error.Description);
                }
                return View(registerViewModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            //We are using viewModel to keep track of the data in case the user accidently reload the page. 
            ViewData["ReturnUrl"] = returnUrl;
            var response = new LoginViewModel();
            return View(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, [FromQuery] string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(loginViewModel);

            //Check if the user exists by using the email address
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null)
            {
                // User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginViewModel.Password);
                if (passwordCheck)
                {
                    // Password correct, Sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User account {loginViewModel.Email} has logged in.");
                        return LocalRedirect("/Home/Index");
                    }
                }
                else
                    ModelState.AddModelError(nameof(loginViewModel.Password), $"The provided password is not correct.");
                return View(loginViewModel);

            }
            else
                ModelState.AddModelError(nameof(loginViewModel.Email), $"User account {loginViewModel.Email} not found.");
            return View(loginViewModel);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return View(model);
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailService.SendEmailAsync(model.Email, "Réinitialiser le mot de passe",
                    $"Veuillez réinitialiser votre mot de passe en cliquant ici: <a href='{callbackUrl}'>Réinitialiser le mot de passe</a>");

                model.EmailSent = true;
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(long userId, string? code = null)
        {
            if (code == null || userId == 0)
            {
                return BadRequest("Un code et un ID utilisateur doivent être fournis pour la réinitialisation du mot de passe.");
            }
            else
            {
                var model = new ResetPasswordViewModel { Token = code, UserId = userId };
                return View(model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            model.Token = model.Token.Replace(' ', '+');
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ForgotPassword), "Account");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                model.IsSuccess = true;
                return View(model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        public async Task<IActionResult> SeeProfile()
        {
            var userInfos = await dbc.Users.FindAsync(GetUser().Id);

            if (userInfos != null)
            {
                var userProfileViewModel = new UserProfileViewModel
                {
                    FirstName = userInfos.FirstName,
                    LastName = userInfos.LastName,
                    PhoneNumber = userInfos.PhoneNumber,
                    CountryCode = userInfos.CountryCode,
                    Email = userInfos.Email,
                };
                return View(userProfileViewModel);
            }
            else
                return NotFound();
        }


        public async Task<Boolean> CheckEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null ? true: false;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(string firstName, string lastName, string countryCode ,string? phoneNumber, string email)
        {
            try
            {
                var currentUser = GetUser();
                if (currentUser != null)
                {
                    if (!String.IsNullOrEmpty(phoneNumber) && !String.IsNullOrEmpty(email))
                    {
                        if (currentUser.Email != email)
                        {
                            if (await CheckEmail(email))
                                return BadRequest("The email already exists");//The email already exists 
                        }
                        if (currentUser.PhoneNumber != phoneNumber || currentUser.CountryCode != countryCode)
                        {
                            var isPhoneNumberTaken = await CheckPhoneNumberAvailability(countryCode,phoneNumber);
                            if (isPhoneNumberTaken.Value.Equals(1))
                                return BadRequest("Phone number already taken"); //The phone number already exists 
                        }
                        await changeProfile(currentUser, firstName, lastName,countryCode, phoneNumber, email);
                        return Json(new { redirectTo = Url.Action(nameof(SeeProfile)) });
                    }
                    else if (String.IsNullOrEmpty(phoneNumber) && !String.IsNullOrEmpty(email))
                    {
                        if (currentUser.Email != email)
                        {
                            if (await CheckEmail(email))
                                return BadRequest("The email already exists"); //The email already exists
                        }
                        await changeProfile(currentUser, firstName, lastName, null, null, email);
                        return Json(new { redirectTo = Url.Action(nameof(SeeProfile)) });
                    }
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task changeProfile(ApplicationUser currentUser, string firstName, string lastName,string? countryCode, string? phoneNumber, string email)
        {
            currentUser.FirstName = firstName;
            currentUser.LastName = lastName;
            currentUser.CountryCode = countryCode;
            currentUser.PhoneNumber = phoneNumber;
            currentUser.Email = email;
            currentUser.UserName = email;
            currentUser.NormalizedEmail = _userManager.NormalizeEmail(email).ToUpper();
            currentUser.NormalizedUserName = _userManager.NormalizeName(email).ToUpper();
            dbc.Users.Update(currentUser);
            await dbc.SaveChangesAsync();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            foreach (var cookie in HttpContext.Request.Cookies)
                Response.Cookies.Delete(cookie.Key);

            _logger.LogInformation($"User {GetUser().UserName} logget out");
            return LocalRedirect("/Account/Login");
        }
    }
}
