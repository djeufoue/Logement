using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Logement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            //We are using viewModel to keep track of the data in case the user accidently reload the page. 
            ViewData["ReturnUrl"] = returnUrl;
            var response = new LoginViewModel();
            return View(response);
        }


        [HttpPost]
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
                    var result = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User account {loginViewModel.Email} has logged in.");

                        if (string.IsNullOrEmpty(returnUrl))
                        {
                            if (await _userManager.IsInRoleAsync(user, "Admin"))
                                returnUrl = "/Admin/Index";
                            else
                                returnUrl = "/";
                        }

                        return LocalRedirect(returnUrl);
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
    }
}
