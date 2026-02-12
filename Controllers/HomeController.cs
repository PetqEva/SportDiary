using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportDiary.Data.Models;
using SportDiary.Services.Interfaces;
using SportDiary.ViewModels.Home;

namespace SportDiary.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IHomeDashboardService _dashboardService;

        public HomeController(
            IHomeDashboardService dashboardService,
            UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new HomeDashboardVm
            {
                IsAuthenticated = IsAuthenticated(),
                Email = User.Identity?.Name
            };

            if (!vm.IsAuthenticated)
                return View(vm);

            var userId = GetUserId();
            var data = await _dashboardService.BuildAsync(userId);

            // запазваме Email от контролера
            data.Email = vm.Email;
            return View(data);
        }

        public IActionResult Privacy() => View();
    }
}
