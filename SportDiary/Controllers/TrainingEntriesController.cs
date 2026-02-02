using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;

namespace SportDiary.Controllers
{
    public class TrainingEntriesController : Controller
    {
        private readonly AppDbContext _context;

        public TrainingEntriesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
        }

        {
            {
            }

            {
            }

        {
        }

        {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainingDiaryId"] = new SelectList(_context.TrainingDiaries, "Id", "Id", trainingEntry.TrainingDiaryId);
            return View(trainingEntry);
        }

        {

            {
            }
            ViewData["TrainingDiaryId"] = new SelectList(_context.TrainingDiaries, "Id", "Id", trainingEntry.TrainingDiaryId);
            return View(trainingEntry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        {
            {
            }

                    {
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
            ViewData["TrainingDiaryId"] = new SelectList(_context.TrainingDiaries, "Id", "Id", trainingEntry.TrainingDiaryId);
            return View(trainingEntry);
        }

            {

            {

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            _context.TrainingEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        {
        }

        private Task<bool> DiaryExists(int diaryId)
            => _context.TrainingDiaries.AsNoTracking().AnyAsync(d => d.Id == diaryId);
    }
}
