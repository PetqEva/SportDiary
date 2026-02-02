using System.ComponentModel.DataAnnotations;
using static SportDiary.Common.ValidationConstants;

namespace SportDiary.ViewModels.TrainingDiaries
{
    public class TrainingDiaryFormViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(DiaryNotesMaxLength)]
        public string? Notes { get; set; }

        [Required]
        public int UserProfileId { get; set; }
    }
}

