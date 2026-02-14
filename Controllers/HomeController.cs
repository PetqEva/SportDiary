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
            UserManager<ApplicationUser> userManager,
            IUserProfileService profileService)
            : base(userManager, profileService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return View(new HomeDashboardVm
                {
                    IsAuthenticated = false
                });
            }

            var userId = GetUserId();
            var vm = await _dashboardService.BuildAsync(userId);

            vm.IsAuthenticated = true;
            vm.Email = User.Identity.Name;

            return View(vm);
        }

        public IActionResult Privacy() => View();
    }
}
