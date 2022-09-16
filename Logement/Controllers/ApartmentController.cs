using Logement.Data;
using Logement.Models;
using Logement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using NuGet.Protocol.Core.Types;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;

namespace Logement.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public ApartmentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
