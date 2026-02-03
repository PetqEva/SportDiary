using Microsoft.AspNetCore.Identity;

namespace SportDiary.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserProfile? UserProfile { get; set; }
    }
}
