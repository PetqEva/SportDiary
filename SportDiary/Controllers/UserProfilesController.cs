using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;
using SportDiary.ViewModels.UserProfiles;

namespace SportDiary.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly AppDbContext _context;

        public UserProfilesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /UserProfiles
        public async Task<IActionResult> Index()
        {
            var users = await _context.UserProfiles
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .Select(u => new UserProfileAllViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age
                })
                .ToListAsync();

            return View(users);
        }

        // GET: /UserProfiles/Create
        public IActionResult Create() => View(new UserProfileFormViewModel());

        // POST: /UserProfiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfileFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var entity = new UserProfile
            {
                Name = model.Name,
                Age = model.Age
            };

            _context.UserProfiles.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /UserProfiles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.UserProfiles.FindAsync(id);
            if (user == null) return NotFound();

            var model = new UserProfileFormViewModel
            {
                Name = user.Name,
                Age = user.Age
            };

            return View(model);
        }

        // POST: /UserProfiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserProfileFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.UserProfiles.FindAsync(id);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Age = model.Age;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /UserProfiles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var vm = new UserProfileAllViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Age = user.Age
            };

            return View(vm);
        }

        // POST: /UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.UserProfiles.FindAsync(id);
            if (user == null) return NotFound();

            _context.UserProfiles.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /UserProfiles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.UserProfiles
                .AsNoTracking()
                .Where(u => u.Id == id)
                .Select(u => new UserProfileAllViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Age = u.Age
                })
                .FirstOrDefaultAsync();

            if (user == null) return NotFound();

            return View(user);
        }

    }
}
