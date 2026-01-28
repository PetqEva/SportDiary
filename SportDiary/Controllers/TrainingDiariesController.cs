using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Models;

namespace SportDiary.Controllers
{
    public class TrainingDiariesController : Controller
    {
        private readonly AppDbContext _context;

        public TrainingDiariesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TrainingDiaries
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.TrainingDiaries.Include(t => t.UserProfile);
            return View(await appDbContext.ToListAsync());
        }

        // GET: TrainingDiaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingDiary = await _context.TrainingDiaries
                .Include(t => t.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainingDiary == null)
            {
                return NotFound();
            }

            return View(trainingDiary);
        }

        // GET: TrainingDiaries/Create
        public IActionResult Create()
        {
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Name");
            return View();
        }

        // POST: TrainingDiaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Notes,UserProfileId")] TrainingDiary trainingDiary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingDiary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Name", trainingDiary.UserProfileId);
            return View(trainingDiary);
        }

        // GET: TrainingDiaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingDiary = await _context.TrainingDiaries.FindAsync(id);
            if (trainingDiary == null)
            {
                return NotFound();
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Name", trainingDiary.UserProfileId);
            return View(trainingDiary);
        }

        // POST: TrainingDiaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Notes,UserProfileId")] TrainingDiary trainingDiary)
        {
            if (id != trainingDiary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingDiary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingDiaryExists(trainingDiary.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Name", trainingDiary.UserProfileId);
            return View(trainingDiary);
        }

        // GET: TrainingDiaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingDiary = await _context.TrainingDiaries
                .Include(t => t.UserProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainingDiary == null)
            {
                return NotFound();
            }

            return View(trainingDiary);
        }

        // POST: TrainingDiaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainingDiary = await _context.TrainingDiaries.FindAsync(id);
            if (trainingDiary != null)
            {
                _context.TrainingDiaries.Remove(trainingDiary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingDiaryExists(int id)
        {
            return _context.TrainingDiaries.Any(e => e.Id == id);
        }
    }
}
