using System.ComponentModel.DataAnnotations;

namespace Quran_Memorizing_System.Models
{
    public class Question
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // "mcq" or "text"

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        // For MCQ questions
        public virtual ICollection<Choice>? Choices { get; set; }

        public string? CorrectAnswerText { get; set; }
    }
}
