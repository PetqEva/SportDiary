using System.ComponentModel.DataAnnotations;
using static SportDiary.Models.ValidationConstants;

namespace SportDiary.Models
{
    public class TrainingDiary
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } // пази само дата

        [StringLength(DiaryNotesMaxLength)]
        public string? Notes { get; set; }

        [Required]
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; } = null!;

        public ICollection<TrainingEntry> TrainingEntries { get; set; } = new List<TrainingEntry>();
    }
}
