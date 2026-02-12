using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SportDiary.Data.Models;

namespace SportDiary.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly UserManager<ApplicationUser> userManager;

        protected BaseController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        protected string GetUserId()
            => userManager.GetUserId(User)!;

        protected bool IsAuthenticated()
            => User?.Identity?.IsAuthenticated ?? false;
    }
}
