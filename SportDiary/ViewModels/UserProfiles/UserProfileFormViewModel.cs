using System.ComponentModel.DataAnnotations;
using static SportDiary.Common.ValidationConstants;

namespace SportDiary.ViewModels.UserProfiles
{
    public class UserProfileFormViewModel
    {
        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;

        [Range(AgeMin, AgeMax)]
        public int Age { get; set; }
    }
}
