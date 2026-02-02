using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;

namespace SportDiary.Controllers
{
    public class TrainingDiariesController : Controller
    {
        private readonly AppDbContext _context;

        public TrainingDiariesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
        }

        {
            }

            {
            }

        }

        {

        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingDiary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

            }

            {
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Name", trainingDiary.UserProfileId);
            return View(trainingDiary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        {
            {
            }

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
                    _context.Update(trainingDiary);
                    await _context.SaveChangesAsync();
                }
                    {
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["UserProfileId"] = new SelectList(_context.UserProfiles, "Id", "Name", trainingDiary.UserProfileId);
            return View(trainingDiary);
        }

            {

            {

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            _context.TrainingDiaries.Remove(diary);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        {
        }

        private Task<bool> UserExists(int userId)
            => _context.UserProfiles.AsNoTracking().AnyAsync(u => u.Id == userId);
    }
}

