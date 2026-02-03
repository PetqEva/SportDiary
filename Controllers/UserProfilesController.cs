using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;
using SportDiary.Data.Models;

namespace SportDiary.Controllers
{
    [Authorize]
    public class UserProfilesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfilesController(AppDbContext context, UserManager<ApplicationUser> userManager)
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

        // GET: UserProfiles/Me  (или просто Index)
        public async Task<IActionResult> Me()
        {
            var profile = await _context.UserProfiles
                .AsNoTracking()
                .Include(p => p.TrainingDiaries.OrderByDescending(d => d.Date))
                .SingleAsync(p => p.IdentityUserId == _userManager.GetUserId(User));

            return View(profile);
        }

        // GET: UserProfiles/Edit
        public async Task<IActionResult> Edit()
        {
            var profile = await GetMyProfileAsync();
            return View(profile);
        }

        // POST: UserProfiles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Name,Age")] UserProfile form)
        {
            var profile = await GetMyProfileAsync();

            if (!ModelState.IsValid)
                return View(profile); // връщаме реалния модел, не "form"

            profile.Name = form.Name;
            profile.Age = form.Age;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Me));
        }
    }
}
