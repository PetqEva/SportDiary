using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportDiary.Data.Models;
using SportDiary.Services.Interfaces;

namespace SportDiary.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IHomeDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            IHomeDashboardService dashboardService,
            UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Ако не е логнат -> показваме публична начална
            if (User.Identity?.IsAuthenticated != true)
                return View();

            // Ако е логнат -> dashboard
            var userId = _userManager.GetUserId(User)!;
            var vm = await _dashboardService.BuildAsync(userId);

            return View(vm);
        }

        public IActionResult Privacy() => View();
    }
}
