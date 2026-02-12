using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportDiary.Data.Models;
using SportDiary.Services.Interfaces;
using SportDiary.ViewModels.TrainingEntries;

namespace SportDiary.Controllers
{
    [Authorize]
    public class TrainingEntriesController : BaseController

    {
        private readonly ITrainingEntryService _entryService;
        private readonly IUserProfileService _profileService;
        
        public TrainingEntriesController(
            ITrainingEntryService entryService,
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
            : base(userManager)
        {
            _entryService = entryService;
            _profileService = profileService;
        }


        private async Task<int> GetMyProfileIdAsync()
        {
            var profile = await _profileService.GetMyProfileAsync(GetUserId());
            return profile.Id;
        }

        private async Task<List<SelectListItem>> BuildMyDiariesSelectAsync(int userProfileId, int? selectedId = null)
        {
            var diaries = await _entryService.GetMyDiarySelectItemsAsync(userProfileId);

            return diaries
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.Text,
                    Selected = selectedId.HasValue && d.Id == selectedId.Value
                })
                .ToList();
        }

        private static void ApplyBusinessRules(TrainingEntryFormVm vm, ModelStateDictionary modelState)
        {
            // Фитнес => без разстояние
            if (!string.IsNullOrWhiteSpace(vm.SportName) &&
                vm.SportName.Trim().Equals("фитнес", StringComparison.OrdinalIgnoreCase) &&
                vm.DistanceKm.HasValue && vm.DistanceKm.Value > 0)
            {
                modelState.AddModelError(nameof(vm.DistanceKm), "За фитнес не се въвежда разстояние.");
            }

            // Без отрицателни стойности
            if (vm.DistanceKm.HasValue && vm.DistanceKm.Value < 0)
            {
                modelState.AddModelError(nameof(vm.DistanceKm), "Разстоянието не може да е отрицателно.");
            }
        }

        public async Task<IActionResult> Index()
        {
            var userProfileId = await GetMyProfileIdAsync();
            var entries = await _entryService.GetMyEntriesAsync(userProfileId);
            return View(entries);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            var entry = await _entryService.GetMyEntryDetailsAsync(id.Value, userProfileId);

            if (entry == null) return NotFound();

            return View(entry);
        }

        // GET: TrainingEntries/Create
        public async Task<IActionResult> Create()
        {
            var userProfileId = await GetMyProfileIdAsync();

            var vm = new TrainingEntryFormVm
            {
                Diaries = await BuildMyDiariesSelectAsync(userProfileId)
            };

            return View(vm);
        }

        // POST: TrainingEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingEntryFormVm vm)
        {
            var userProfileId = await GetMyProfileIdAsync();

            ApplyBusinessRules(vm, ModelState);

            if (!ModelState.IsValid)
            {
                vm.Diaries = await BuildMyDiariesSelectAsync(userProfileId, vm.TrainingDiaryId);
                return View(vm);
            }

            var entity = new TrainingEntry
            {
                SportName = vm.SportName.Trim(),
                DurationMinutes = vm.DurationMinutes,
                Calories = vm.Calories,
                DistanceKm = vm.DistanceKm,
                TrainingDiaryId = vm.TrainingDiaryId
            };

            try
            {
                await _entryService.CreateAsync(entity, userProfileId);
            }
            catch (UnauthorizedAccessException)
            {
                // по-добро UX от гол 404
                ModelState.AddModelError(nameof(vm.TrainingDiaryId), "Невалиден дневник.");
                vm.Diaries = await BuildMyDiariesSelectAsync(userProfileId, vm.TrainingDiaryId);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            var entry = await _entryService.GetMyEntryForEditAsync(id.Value, userProfileId);

            if (entry == null) return NotFound();

            var vm = new TrainingEntryFormVm
            {
                Id = entry.Id,
                SportName = entry.SportName,
                DurationMinutes = entry.DurationMinutes,
                Calories = entry.Calories,
                DistanceKm = entry.DistanceKm,
                TrainingDiaryId = entry.TrainingDiaryId,
                Diaries = await BuildMyDiariesSelectAsync(userProfileId, entry.TrainingDiaryId)
            };

            return View(vm);
        }

        // POST: TrainingEntries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingEntryFormVm vm)
        {
            if (id != vm.Id) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();

            ApplyBusinessRules(vm, ModelState);

            if (!ModelState.IsValid)
            {
                vm.Diaries = await BuildMyDiariesSelectAsync(userProfileId, vm.TrainingDiaryId);
                return View(vm);
            }

            var entity = new TrainingEntry
            {
                Id = vm.Id,
                SportName = vm.SportName.Trim(),
                DurationMinutes = vm.DurationMinutes,
                Calories = vm.Calories,
                DistanceKm = vm.DistanceKm,
                TrainingDiaryId = vm.TrainingDiaryId
            };

            var updated = await _entryService.UpdateAsync(entity, userProfileId);
            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            var entry = await _entryService.GetMyEntryDetailsAsync(id.Value, userProfileId);

            if (entry == null) return NotFound();

            return View(entry);
        }

        // POST: TrainingEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfileId = await GetMyProfileIdAsync();
            await _entryService.DeleteAsync(id, userProfileId);
            return RedirectToAction(nameof(Index));
        }
    }
}
