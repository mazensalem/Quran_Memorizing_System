using System.ComponentModel.DataAnnotations;

namespace Quran_Memorizing_System.Models
{
    public class Exam
    {
        public int Exam_ID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Sheikh_email { get; set; }

        public int? Circle_ID { get; set; }

        [Required]
        public DateTime starttime { get; set; }

        [Required]
        public DateTime endtime { get; set; }

        [Required]
        public int examduration { get; set; } // Duration in minutes

        public bool PublicAvailabilty { get; set; }

        // Navigation property
        public virtual ICollection<Question> Questions { get; set; }
    }
}
