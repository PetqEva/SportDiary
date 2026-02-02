using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;
using SportDiary.ViewModels.TrainingEntries;

namespace SportDiary.Controllers
{
    public class TrainingEntriesController : Controller
    {
        private readonly AppDbContext _context;

        public TrainingEntriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /TrainingEntries
        public async Task<IActionResult> Index()
        {
            var entries = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile)
                .OrderByDescending(e => e.Id)
                .Select(e => new TrainingEntryAllViewModel
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm,
                    TrainingDiaryId = e.TrainingDiaryId,
                    DiaryLabel = e.TrainingDiary.UserProfile.Name + " - " + e.TrainingDiary.Date.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            return View(entries);
        }

        // GET: /TrainingEntries/Create
        public async Task<IActionResult> Create(int? diaryId)
        {
            await LoadDiariesAsync();

            return View(new TrainingEntryFormViewModel
            {
                TrainingDiaryId = diaryId ?? 0
            });
        }


        // POST: /TrainingEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingEntryFormViewModel model)
        {
            if (!await DiaryExists(model.TrainingDiaryId))
            {
                ModelState.AddModelError(nameof(model.TrainingDiaryId), "Невалиден дневник.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDiariesAsync();
                return View(model);
            }

            var entity = new TrainingEntry
            {
                SportName = model.SportName,
                DurationMinutes = model.DurationMinutes,
                Calories = model.Calories,
                DistanceKm = model.DistanceKm,
                TrainingDiaryId = model.TrainingDiaryId
            };

            _context.TrainingEntries.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /TrainingEntries/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null) return NotFound();

            var model = new TrainingEntryFormViewModel
            {
                SportName = entry.SportName,
                DurationMinutes = entry.DurationMinutes,
                Calories = entry.Calories,
                DistanceKm = entry.DistanceKm,
                TrainingDiaryId = entry.TrainingDiaryId
            };

            await LoadDiariesAsync();
            return View(model);
        }

        // POST: /TrainingEntries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingEntryFormViewModel model)
        {
            if (!await DiaryExists(model.TrainingDiaryId))
            {
                ModelState.AddModelError(nameof(model.TrainingDiaryId), "Невалиден дневник.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDiariesAsync();
                return View(model);
            }

            var entry = await _context.TrainingEntries.FindAsync(id);
            if (entry == null) return NotFound();

            entry.SportName = model.SportName;
            entry.DurationMinutes = model.DurationMinutes;
            entry.Calories = model.Calories;
            entry.DistanceKm = model.DistanceKm;
            entry.TrainingDiaryId = model.TrainingDiaryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /TrainingEntries/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (entry == null) return NotFound();

            var vm = new TrainingEntryAllViewModel
            {
                Id = entry.Id,
                SportName = entry.SportName,
                DurationMinutes = entry.DurationMinutes,
                Calories = entry.Calories,
                DistanceKm = entry.DistanceKm,
                TrainingDiaryId = entry.TrainingDiaryId,
                DiaryLabel = entry.TrainingDiary.UserProfile.Name + " - " + entry.TrainingDiary.Date.ToString("yyyy-MM-dd")
            };

            return View(vm);
        }

        // POST: /TrainingEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entry = await _context.TrainingEntries.FindAsync(id);
            if (entry == null) return NotFound();

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /TrainingEntries/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                    .ThenInclude(d => d.UserProfile)
                .Where(e => e.Id == id)
                .Select(e => new SportDiary.ViewModels.TrainingEntries.TrainingEntryDetailsViewModel
                {
                    Id = e.Id,
                    SportName = e.SportName,
                    DurationMinutes = e.DurationMinutes,
                    Calories = e.Calories,
                    DistanceKm = e.DistanceKm,

                    TrainingDiaryId = e.TrainingDiaryId,
                    DiaryDate = e.TrainingDiary.Date,
                    UserProfileId = e.TrainingDiary.UserProfileId,
                    UserName = e.TrainingDiary.UserProfile.Name,
                    DiaryLabel = e.TrainingDiary.UserProfile.Name + " - " + e.TrainingDiary.Date.ToString("yyyy-MM-dd")
                })
                .FirstOrDefaultAsync();

            if (entry == null) return NotFound();

            return View(entry);
        }

        private async Task LoadDiariesAsync()
        {
            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.UserProfile)
                .OrderByDescending(d => d.Date)
                .Select(d => new
                {
                    d.Id,
                    Label = d.UserProfile.Name + " - " + d.Date.ToString("yyyy-MM-dd")
                })
                .ToListAsync();

            ViewBag.TrainingDiaries = new SelectList(diaries, "Id", "Label");
        }

        private Task<bool> DiaryExists(int diaryId)
            => _context.TrainingDiaries.AsNoTracking().AnyAsync(d => d.Id == diaryId);
    }
}
