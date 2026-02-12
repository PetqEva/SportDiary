using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportDiary.Data.Models;
using SportDiary.Services.Interfaces;
using SportDiary.ViewModels.TrainingDiaries;

namespace SportDiary.Controllers
{
    [Authorize]
    public class TrainingDiariesController : Controller
    {
        private readonly ITrainingDiaryService _diaryService;
        private readonly IUserProfileService _profileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainingDiariesController(
            ITrainingDiaryService diaryService,
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
        {
            _diaryService = diaryService;
            _profileService = profileService;
            _userManager = userManager;
        }

        private string GetUserId() => _userManager.GetUserId(User)!;

        private async Task<int> GetMyProfileIdAsync()
        {
            var userId = GetUserId();
            var profile = await _profileService.GetMyProfileAsync(userId);
            return profile.Id;
        }

        private static List<SelectListItem> BuildPlaceOptions() => new()
        {
            new SelectListItem { Value = "Home", Text = "В къщи" },
            new SelectListItem { Value = "Gym", Text = "Фитнес" },
            new SelectListItem { Value = "Outdoor", Text = "Навън" },
            new SelectListItem { Value = "Other", Text = "Друго" }
        };

        // GET: TrainingDiaries
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userProfileId = await GetMyProfileIdAsync();
            var diaries = await _diaryService.GetMyDiariesAsync(userProfileId);
            return View(diaries);
        }

        // GET: TrainingDiaries/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            var diary = await _diaryService.GetMyDiaryDetailsAsync(id.Value, userProfileId);

            if (diary == null) return NotFound();

            return View(diary);
        }

        // GET: TrainingDiaries/Create
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new TrainingDiaryFormVm
            {
                Date = DateTime.Today,
                PlaceOptions = BuildPlaceOptions()
            };

            return View(vm);
        }

        // POST: TrainingDiaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingDiaryFormVm vm)
        {
            var userProfileId = await GetMyProfileIdAsync();

            if (!ModelState.IsValid)
            {
                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }

            var exists = await _diaryService.DiaryExistsForDateAsync(userProfileId, vm.Date);
            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Date), "Вече има дневник за тази дата.");
                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }

            var entity = new TrainingDiary
            {
                UserProfileId = userProfileId,
                Date = vm.Date,
                DurationMinutes = vm.DurationMinutes,
                Place = vm.Place,
                WaterLiters = vm.WaterLiters,
                Notes = vm.Notes
            };

            await _diaryService.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingDiaries/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            var diary = await _diaryService.GetMyDiaryForEditAsync(id.Value, userProfileId);

            if (diary == null) return NotFound();

            var vm = new TrainingDiaryFormVm
            {
                Id = diary.Id,
                Date = diary.Date,
                DurationMinutes = diary.DurationMinutes,
                Place = diary.Place,
                WaterLiters = diary.WaterLiters,
                Notes = diary.Notes,
                PlaceOptions = BuildPlaceOptions()
            };

            return View(vm);
        }

        // POST: TrainingDiaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingDiaryFormVm vm)
        {
            if (id != vm.Id) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();

            if (!ModelState.IsValid)
            {
                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }

            var exists = await _diaryService.DiaryExistsForDateAsync(
                userProfileId, vm.Date, excludeDiaryId: vm.Id);

            if (exists)
            {
                ModelState.AddModelError(nameof(vm.Date), "Вече има дневник за тази дата.");
                vm.PlaceOptions = BuildPlaceOptions();
                return View(vm);
            }

            var updated = await _diaryService.UpdateAsync(
                vm.Id,
                userProfileId,
                vm.Date,
                vm.DurationMinutes,
                vm.Place,
                vm.WaterLiters,
                vm.Notes
            );

            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingDiaries/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            var diary = await _diaryService.GetMyDiaryDetailsAsync(id.Value, userProfileId);

            if (diary == null) return NotFound();

            return View(diary);
        }

        // POST: TrainingDiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfileId = await GetMyProfileIdAsync();

            var deleted = await _diaryService.DeleteAsync(id, userProfileId);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
