using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;
using SportDiary.Services.Core.Interfaces;
using SportDiary.ViewModels.Home;

namespace SportDiary.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(
            AppDbContext context,
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _profileService = profileService;
            _userManager = userManager;
        }

        private string? GetUserId() => _userManager.GetUserId(User);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new HomeDashboardVm
            {
                IsAuthenticated = User.Identity?.IsAuthenticated == true,
                Email = User.Identity?.Name
            };

            if (!vm.IsAuthenticated)
                return View(vm);

            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
                return View(vm);

            // Профил
            var profile = await _profileService.GetMyProfileAsync(userId);
            vm.ProfileName = profile.Name;

            // KPI
            vm.DiariesCount = await _context.TrainingDiaries.CountAsync(d => d.UserProfileId == profile.Id);
            vm.EntriesCount = await _context.TrainingEntries
                    .Where(e => e.TrainingDiary.UserProfileId == profile.Id)
                    .CountAsync();
            vm.TotalDurationMinutes = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profile.Id)
                .SumAsync(d => (int?)d.DurationMinutes) ?? 0;

            vm.TotalWaterLiters = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profile.Id)
                .SumAsync(d => (double?)d.WaterLiters) ?? 0;

            // Последни 5 дневника
            vm.RecentDiaries = await _context.TrainingDiaries
                .Where(d => d.UserProfileId == profile.Id)
                .OrderByDescending(d => d.Date)
                .Take(5)
                .Select(d => new HomeDashboardVm.RecentDiaryVm
                {
                    Id = d.Id,
                    Date = d.Date,
                    Place = d.Place,
                    DurationMinutes = d.DurationMinutes,
                    WaterLiters = d.WaterLiters,
                    Notes = d.Notes
                })
                .ToListAsync();

            return View(vm);
        }

        public IActionResult Privacy() => View();
    }
}
