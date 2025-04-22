using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishLearningAPI.Models
{
    [Table("user_progress")]
    public class UserProgress
    {
        [Key]
        [Column("progress_id")]
        public int Id { get; set; }

        [Column("last_reviewed")]
        public DateTime? LastReviewed { get; set; }

        [Column("next_review")]
        public DateTime? NextReview { get; set; }

        [Column("proficiency_level")]
        public int Level { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int? UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Vocabulary")]
        [Column("vocab_id")]
        public int? VocabularyId { get; set; }
        public virtual Vocabulary Vocabulary { get; set; }

        public UserProgress()
        {
        }

        public UserProgress(DateTime? lastReviewed, DateTime? nextReview)
        {
            LastReviewed = lastReviewed;
            NextReview = nextReview;
        }
    }
}
