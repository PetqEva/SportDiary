using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;

namespace SportDiary.Controllers
{
    [Authorize]
    public class TrainingDiariesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TrainingDiariesController(AppDbContext context, UserManager<ApplicationUser> userManager)
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

        // GET: TrainingDiaries
        public async Task<IActionResult> Index()
        {
            var profile = await GetMyProfileAsync();

            var diaries = await _context.TrainingDiaries
                .AsNoTracking()
                .Where(d => d.UserProfileId == profile.Id)
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            return View(diaries);
        }

        // GET: TrainingDiaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var profile = await GetMyProfileAsync();

            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .Include(d => d.TrainingEntries)
                .FirstOrDefaultAsync(d => d.Id == id && d.UserProfileId == profile.Id);

            if (diary == null) return NotFound();

            return View(diary);
        }

        // GET: TrainingDiaries/Create
        public IActionResult Create()
        {
            // Няма dropdown за UserProfileId в реално приложение.
            return View(new TrainingDiary { Date = DateTime.Today });
        }

        // POST: TrainingDiaries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Date,Notes")] TrainingDiary trainingDiary)
        {
            var profile = await GetMyProfileAsync();

            trainingDiary.Date = trainingDiary.Date.Date;
            trainingDiary.UserProfileId = profile.Id;

            if (!ModelState.IsValid)
                return View(trainingDiary);

            // Guard: one diary per user per date.
            var exists = await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.UserProfileId == profile.Id && d.Date == trainingDiary.Date);

            if (exists)
            {
                ModelState.AddModelError(nameof(TrainingDiary.Date), "Вече има дневник за тази дата.");
                return View(trainingDiary);
            }

            _context.Add(trainingDiary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingDiaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var profile = await GetMyProfileAsync();

            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.UserProfileId == profile.Id);

            if (diary == null) return NotFound();

            return View(diary);
        }

        // POST: TrainingDiaries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Notes")] TrainingDiary form)
        {
            if (id != form.Id) return NotFound();

            var profile = await GetMyProfileAsync();

            // Зареждаме истинския entity от DB (ownership + защита от overposting)
            var diary = await _context.TrainingDiaries
                .FirstOrDefaultAsync(d => d.Id == id && d.UserProfileId == profile.Id);

            if (diary == null) return NotFound();

            form.Date = form.Date.Date;

            if (!ModelState.IsValid)
                return View(form);

            // Guard: unique per date (exclude current)
            var exists = await _context.TrainingDiaries
                .AsNoTracking()
                .AnyAsync(d => d.Id != id && d.UserProfileId == profile.Id && d.Date == form.Date);

            if (exists)
            {
                ModelState.AddModelError(nameof(TrainingDiary.Date), "Вече има дневник за тази дата.");
                return View(form);
            }

            diary.Date = form.Date;
            diary.Notes = form.Notes;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TrainingDiaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var profile = await GetMyProfileAsync();

            var diary = await _context.TrainingDiaries
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id && d.UserProfileId == profile.Id);

            if (diary == null) return NotFound();

            return View(diary);
        }

        // POST: TrainingDiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await GetMyProfileAsync();

            var diary = await _context.TrainingDiaries
                .FirstOrDefaultAsync(d => d.Id == id && d.UserProfileId == profile.Id);

            if (diary == null) return NotFound();

            _context.TrainingDiaries.Remove(diary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
