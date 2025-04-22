using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishLearningAPI.Models
{
    [Table("topic_progress")]
    public class TopicProgress
    {
        [Key]
        [Column("progress_id")]
        public int Id { get; set; }

        [Column("is_completed")]
        public int IsCompleted { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [ForeignKey("User")]
        [Column("user_id")]
        public int? UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Topic")]
        [Column("topic_id")]
        public int? TopicId { get; set; }
        public virtual Topic Topic { get; set; }

        public TopicProgress()
        {
        }

        public TopicProgress(int isCompleted, DateTime? completedAt)
        {
            IsCompleted = isCompleted;
            CompletedAt = completedAt;
        }
    }
}
