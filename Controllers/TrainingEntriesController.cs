using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public TrainingEntriesController(
            ITrainingEntryService entryService,
            IUserProfileService profileService,
            UserManager<ApplicationUser> userManager)
            : base(userManager, profileService)
        {
            _entryService = entryService;
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

        public async Task<IActionResult> Index([FromQuery] EntriesQueryVm query)
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            var vm = new EntriesIndexVm
            {
                Query = query,
                Diaries = await BuildMyDiariesSelectAsync(pid, query.DiaryId),
                Result = await _entryService.GetMyEntriesPagedAsync(pid, query)
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var entry = await _entryService.GetMyEntryDetailsAsync(id.Value, userProfileId.Value);
            if (entry == null) return NotFound();

            return View(entry);
        }

        // GET: TrainingEntries/Create
        public async Task<IActionResult> Create()
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var vm = new TrainingEntryFormVm
            {
                Diaries = await BuildMyDiariesSelectAsync(userProfileId.Value)
            };

            return View(vm);
        }

        // POST: TrainingEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingEntryFormVm vm)
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            foreach (var (field, message) in _entryService.ValidateBusinessRules(vm))
                ModelState.AddModelError(field, message);

            if (!ModelState.IsValid)
            {
                vm.Diaries = await BuildMyDiariesSelectAsync(pid, vm.TrainingDiaryId);
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
                await _entryService.CreateAsync(entity, pid);
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError(nameof(vm.TrainingDiaryId), "Невалиден дневник.");
                vm.Diaries = await BuildMyDiariesSelectAsync(pid, vm.TrainingDiaryId);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            var entry = await _entryService.GetMyEntryForEditAsync(id.Value, pid);
            if (entry == null) return NotFound();

            var vm = new TrainingEntryFormVm
            {
                Id = entry.Id,
                SportName = entry.SportName,
                DurationMinutes = entry.DurationMinutes,
                Calories = entry.Calories,
                DistanceKm = entry.DistanceKm,
                TrainingDiaryId = entry.TrainingDiaryId,
                Diaries = await BuildMyDiariesSelectAsync(pid, entry.TrainingDiaryId)
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
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var pid = userProfileId.Value;

            foreach (var (field, message) in _entryService.ValidateBusinessRules(vm))
                ModelState.AddModelError(field, message);

            if (!ModelState.IsValid)
            {
                vm.Diaries = await BuildMyDiariesSelectAsync(pid, vm.TrainingDiaryId);
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

            var updated = await _entryService.UpdateAsync(entity, pid);
            if (!updated) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var entry = await _entryService.GetMyEntryDetailsAsync(id.Value, userProfileId.Value);
            if (entry == null) return NotFound();

            return View(entry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userProfileId = await GetMyProfileIdAsync();
            if (userProfileId == null) return RedirectToAction("Create", "UserProfiles");

            var deleted = await _entryService.DeleteAsync(id, userProfileId.Value);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
