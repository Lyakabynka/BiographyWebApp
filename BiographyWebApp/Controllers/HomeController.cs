using BiographyWebApp.Abstractions;
using BiographyWebApp.Database.Repositories;
using BiographyWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BiographyWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAppRepository _repo;

        public HomeController(IAppRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> AuthorizedPage()
        {
            return View();
        }

        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> AdminAccount()
        {
            return View();
        }

        [Authorize(Roles = nameof(UserRole.User))]
        public async Task<IActionResult> AccountAsync()
        {
            User user = await _repo.GetUserByEmailAsync(User.Claims.First(u=>u.Type == ClaimTypes.Email).Value);
            return View(user);
        }
    }
}