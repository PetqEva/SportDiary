using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportDiary.Data.Models;
using SportDiary.Services.Interfaces;
using SportDiary.ViewModels.UserProfiles;

namespace SportDiary.Controllers
{
    [Authorize]
    public class UserProfilesController : Controller
    {
        private readonly IUserProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfilesController(
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _userManager = userManager;
        }

        private string GetUserId() => _userManager.GetUserId(User)!;

        private static List<SelectListItem> BuildActivityOptions() => new()
        {
            new SelectListItem { Value = "Low", Text = "Ниска активност" },
            new SelectListItem { Value = "Medium", Text = "Средна активност" },
            new SelectListItem { Value = "High", Text = "Висока активност" }
        };

        [HttpGet]
        public IActionResult Me() => RedirectToAction(nameof(Details));

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileWithDiariesAsync(userId);
            return View(profile);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var vm = new EditUserProfileVm
            {
                ActivityOptions = BuildActivityOptions()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EditUserProfileVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ActivityOptions = BuildActivityOptions();
                return View(vm);
            }

            var userId = GetUserId();

            await _profileService.CreateMyProfileAsync(
                userId,
                vm.Name,
                vm.Age,
                vm.Gender,
                vm.StartWeightKg,
                vm.CurrentWeightKg,
                vm.HeightCm,
                vm.ActivityLevel);

            return RedirectToAction(nameof(Details));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);

            var vm = new EditUserProfileVm
            {
                Name = profile.Name,
                Age = profile.Age,
                Gender = profile.Gender,
                StartWeightKg = profile.StartWeightKg,
                CurrentWeightKg = profile.CurrentWeightKg,
                HeightCm = profile.HeightCm,
                ActivityLevel = profile.ActivityLevel,
                ActivityOptions = BuildActivityOptions()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserProfileVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.ActivityOptions = BuildActivityOptions();
                return View(vm);
            }

            var userId = GetUserId();

            await _profileService.UpdateMyProfileAsync(
                userId,
                vm.Name,
                vm.Age,
                vm.Gender,
                vm.StartWeightKg,
                vm.CurrentWeightKg,
                vm.HeightCm,
                vm.ActivityLevel);

            return RedirectToAction(nameof(Details));
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed()
        {
            var userId = GetUserId();
            await _profileService.DeleteMyProfileAsync(userId);
            return RedirectToAction("Index", "Home");
        }
    }
}
