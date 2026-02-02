using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;
using SportDiary.ViewModels.TrainingDiaries;

namespace SportDiary.Controllers
{
    public class TrainingDiariesController : Controller
    {
        private readonly AppDbContext _context;

        public TrainingDiariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /TrainingDiaries
        public async Task<IActionResult> Index()
        {
            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.UserProfile)
                .OrderByDescending(d => d.Date)
                .Select(d => new TrainingDiaryAllViewModel
                {
                    Id = d.Id,
                    Date = d.Date,
                    Notes = d.Notes,
                    UserProfileId = d.UserProfileId,
                    UserName = d.UserProfile.Name
                })
                .ToListAsync();

            return View(diaries);
        }

        // GET: /TrainingDiaries/Create
        public async Task<IActionResult> Create()
        {
            await LoadUsersAsync();
            return View(new TrainingDiaryFormViewModel { Date = DateTime.Today });
        }

        // POST: /TrainingDiaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainingDiaryFormViewModel model)
        {
            if (!await UserExists(model.UserProfileId))
            {
                ModelState.AddModelError(nameof(model.UserProfileId), "Невалиден потребител.");
            }

            if (!ModelState.IsValid)
            {
                await LoadUsersAsync();
                return View(model);
            }

            var entity = new TrainingDiary
            {
                Date = model.Date.Date,
                Notes = model.Notes,
                UserProfileId = model.UserProfileId
            };

            _context.TrainingDiaries.Add(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Уникален индекс: (UserProfileId + Date) -> 1 дневник на ден
                ModelState.AddModelError(string.Empty, "Вече има дневник за този потребител на тази дата.");
                await LoadUsersAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /TrainingDiaries/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (diary == null) return NotFound();

            var model = new TrainingDiaryFormViewModel
            {
                Date = diary.Date,
                Notes = diary.Notes,
                UserProfileId = diary.UserProfileId
            };

            await LoadUsersAsync();
            return View(model);
        }

        // POST: /TrainingDiaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainingDiaryFormViewModel model)
        {
            if (!await UserExists(model.UserProfileId))
            {
                ModelState.AddModelError(nameof(model.UserProfileId), "Невалиден потребител.");
            }

            if (!ModelState.IsValid)
            {
                await LoadUsersAsync();
                return View(model);
            }

            var diary = await _context.TrainingDiaries.FindAsync(id);
            if (diary == null) return NotFound();

            diary.Date = model.Date.Date;
            diary.Notes = model.Notes;
            diary.UserProfileId = model.UserProfileId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Вече има дневник за този потребител на тази дата.");
                await LoadUsersAsync();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /TrainingDiaries/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.UserProfile)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (diary == null) return NotFound();

            var vm = new TrainingDiaryAllViewModel
            {
                Id = diary.Id,
                Date = diary.Date,
                Notes = diary.Notes,
                UserProfileId = diary.UserProfileId,
                UserName = diary.UserProfile.Name
            };

            return View(vm);
        }

        // POST: /TrainingDiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diary = await _context.TrainingDiaries.FindAsync(id);
            if (diary == null) return NotFound();

            _context.TrainingDiaries.Remove(diary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /TrainingDiaries/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.UserProfile)
                .Include(d => d.TrainingEntries)
                .Where(d => d.Id == id)
                .Select(d => new SportDiary.ViewModels.TrainingDiaries.TrainingDiaryDetailsViewModel
                {
                    Id = d.Id,
                    Date = d.Date,
                    Notes = d.Notes,
                    UserProfileId = d.UserProfileId,
                    UserName = d.UserProfile.Name,
                    Entries = d.TrainingEntries
                        .OrderByDescending(e => e.Id)
                        .Select(e => new SportDiary.ViewModels.TrainingDiaries.TrainingDiaryDetailsViewModel.EntryItem
                        {
                            Id = e.Id,
                            SportName = e.SportName,
                            DurationMinutes = e.DurationMinutes,
                            Calories = e.Calories,
                            DistanceKm = e.DistanceKm
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (diary == null) return NotFound();

            return View(diary);
        }

        private async Task LoadUsersAsync()
        {
            var users = await _context.UserProfiles
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .Select(u => new { u.Id, u.Name })
                .ToListAsync();

            ViewBag.UserProfiles = new SelectList(users, "Id", "Name");
        }

        private Task<bool> UserExists(int userId)
            => _context.UserProfiles.AsNoTracking().AnyAsync(u => u.Id == userId);
    }
}

