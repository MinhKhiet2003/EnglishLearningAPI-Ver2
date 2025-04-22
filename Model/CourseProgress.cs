using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EnglishLearningAPI.Models
{
    [Table("course_progress")]
    public class CourseProgress
    {
        [Key]
        [Column("progress_id")]
        public int Id { get; set; }

        [Column("is_completed")]
        public int IsCompleted { get; set; }

        [Column("completed_at")]
        public DateTime? CompletedAt { get; set; }

        [JsonIgnore]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("Course")]
        [Column("course_id")]
        public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public CourseProgress()
        {
        }

        public CourseProgress(int isCompleted, DateTime? completedAt)
        {
            IsCompleted = isCompleted;
            CompletedAt = completedAt;
        }
    }
}
