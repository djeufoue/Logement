using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Logement.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
             IConfiguration configuration,
            ILogger<AccountController> logger):base(context, configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                PhoneNumber = registerViewModel.PhoneNumber,
                FirstName = registerViewModel.TenantFirstName,
                LastName = registerViewModel.TenantLastName,
                JobTitle = registerViewModel.JobTitle,
                MaritalStatus = registerViewModel.MaritalStatus
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
                        return LocalRedirect("/City/Index");
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
