using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportDiary.Data;
using SportDiary.Data.Models;

namespace SportDiary.Controllers
{
    [Authorize]
    public class NutritionTargetsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public NutritionTargetsController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int calories, int proteinGrams)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // държим последната цел като активна (изтриваме старите)
            var old = _db.NutritionTargets.Where(x => x.UserId == userId);
            _db.NutritionTargets.RemoveRange(old);

            _db.NutritionTargets.Add(new NutritionTarget
            {
                UserId = userId,
                Calories = calories,
                ProteinGrams = proteinGrams
            });

            await _db.SaveChangesAsync();

            TempData["Success"] = "Запазено: дневна калорийна и протеинова цел.";
            return RedirectToAction("Tdee", "Calculators");
        }
    }
}
