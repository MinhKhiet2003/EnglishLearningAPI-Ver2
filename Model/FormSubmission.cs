using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EnglishLearningAPI.Models
{
    [Table("form_submission")]
    public class FormSubmission
    {
        [Key]
        [Column("submission_id")]
        public int Id { get; set; }

        [Column("form_type")]
        public int FormType { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("status")]
        public int Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore] // Để tránh vòng lặp khi serializing JSON
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public FormSubmission()
        {
        }

        public FormSubmission(int formType, string content, int status)
        {
            FormType = formType;
            Content = content;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
