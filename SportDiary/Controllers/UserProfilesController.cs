using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDiary.Data;

namespace SportDiary.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly AppDbContext _context;

        public UserProfilesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
        {

            }


        [HttpPost]
        [ValidateAntiForgeryToken]
        {
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(userProfile);
        }

            {

            {
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        {

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(userProfile.Id))
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
            return View(userProfile);
        }

            {

            {

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            _context.UserProfiles.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        {
        }

    }
}
