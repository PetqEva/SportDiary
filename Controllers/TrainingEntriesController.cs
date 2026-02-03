using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;

namespace SportDiary.Controllers
{
    [Authorize]
    public class TrainingEntriesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainingEntriesController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<UserProfile> GetMyProfileAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("No logged-in user id.");

            return await _context.UserProfiles.SingleAsync(p => p.IdentityUserId == userId);
        }

        // GET: TrainingEntries
        public async Task<IActionResult> Index()
        {
            var profile = await GetMyProfileAsync();

            var entries = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .Where(e => e.TrainingDiary.UserProfileId == profile.Id)
                .OrderByDescending(e => e.Id)
                .ToListAsync();

            return View(entries);
        }

        // GET: TrainingEntries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var profile = await GetMyProfileAsync();

            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingDiary.UserProfileId == profile.Id);

            if (entry == null) return NotFound();

            return View(entry);
        }

        // GET: TrainingEntries/Create
        public async Task<IActionResult> Create()
        {
            await LoadDiariesAsync();
            return View();
        }

        // POST: TrainingEntries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SportName,DurationMinutes,Calories,DistanceKm,TrainingDiaryId")] TrainingEntry trainingEntry)
        {
            var profile = await GetMyProfileAsync();

            // ✅ diaryId трябва да е мой
            var diaryOk = await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.Id == trainingEntry.TrainingDiaryId && d.UserProfileId == profile.Id);

            if (!diaryOk)
                return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadDiariesAsync(trainingEntry.TrainingDiaryId);
                return View(trainingEntry);
            }

            _context.Add(trainingEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingEntries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var profile = await GetMyProfileAsync();

            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingDiary.UserProfileId == profile.Id);

            if (entry == null) return NotFound();

            await LoadDiariesAsync(entry.TrainingDiaryId);
            return View(entry);
        }

        // POST: TrainingEntries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SportName,DurationMinutes,Calories,DistanceKm,TrainingDiaryId")] TrainingEntry form)
        {
            if (id != form.Id) return NotFound();

            var profile = await GetMyProfileAsync();

            // ✅ entity от DB (ownership + защита от overposting)
            var entry = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingDiary.UserProfileId == profile.Id);

            if (entry == null) return NotFound();

            // ✅ diaryId трябва да е мой (ако потребителят го сменя)
            var diaryOk = await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.Id == form.TrainingDiaryId && d.UserProfileId == profile.Id);

            if (!diaryOk)
                return Forbid();

            if (!ModelState.IsValid)
            {
                await LoadDiariesAsync(form.TrainingDiaryId);
                return View(form);
            }

            entry.SportName = form.SportName;
            entry.DurationMinutes = form.DurationMinutes;
            entry.Calories = form.Calories;
            entry.DistanceKm = form.DistanceKm;
            entry.TrainingDiaryId = form.TrainingDiaryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var profile = await GetMyProfileAsync();

            var entry = await _context.TrainingEntries
                .AsNoTracking()
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingDiary.UserProfileId == profile.Id);

            if (entry == null) return NotFound();

            return View(entry);
        }

        // POST: TrainingEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await GetMyProfileAsync();

            var entry = await _context.TrainingEntries
                .Include(e => e.TrainingDiary)
                .FirstOrDefaultAsync(e => e.Id == id && e.TrainingDiary.UserProfileId == profile.Id);

            if (entry == null) return NotFound();

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDiariesAsync(int? selectedDiaryId = null)
        {
            var profile = await GetMyProfileAsync();

            // ✅ само моите дневници
            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == profile.Id)
                .OrderByDescending(d => d.Date)
                .Select(d => new
                {
                    d.Id,
                    Text = $"#{d.Id} | {d.Date:yyyy-MM-dd}"
                })
                .ToListAsync();

            ViewBag.TrainingDiaryId = new SelectList(diaries, "Id", "Text", selectedDiaryId);
        }
    }
}
