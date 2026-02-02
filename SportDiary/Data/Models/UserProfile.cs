using System.ComponentModel.DataAnnotations;
using static SportDiary.Common.ValidationConstants;

namespace SportDiary.Data.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = string.Empty;

        [Range(AgeMin, AgeMax)]
        public int Age { get; set; }

        public ICollection<TrainingDiary> TrainingDiaries { get; set; } = new List<TrainingDiary>();
    }
}
