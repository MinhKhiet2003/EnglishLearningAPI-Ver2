using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace EnglishLearningAPI.Models
{
    [Table("user")]
    public class User 
    {
        [Key]
        [Column("user_id")]
        [JsonPropertyName("userId")]
        public int Id { get; set; }

        [Required]
        [Column("email")]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [Column("password")]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [Column("Salt")]
        [MaxLength(255)]
        public string Salt { get; set; }
        [Required]
        [Column("full_name")]
        [MaxLength(255)]
        public string FullName { get; set; }

        [Column("subscription_plan")]
        [MaxLength(20)]
        public string SubscriptionPlan { get; set; }

        [Column("subscription_start_date")]
        public DateTime? SubscriptionStartDate { get; set; }

        [Column("subscription_end_date")]
        public DateTime? SubscriptionEndDate { get; set; }

        [Column("role")]
        [MaxLength(20)]
        public string Role { get; set; }

        [Column("paid")]
        public bool Paid { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navigation Properties

        public virtual ICollection<UserProgress> UserProgresses { get; set; } = new List<UserProgress>();
        public virtual ICollection<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
        public virtual ICollection<TopicProgress> TopicProgresses { get; set; } = new List<TopicProgress>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [JsonIgnore]
        public virtual ICollection<FormSubmission> FormSubmissions { get; set; } = new List<FormSubmission>();

        // Methods for adding related entities

        public void AddUserProgress(UserProgress userProgress)
        {
            UserProgresses.Add(userProgress);
            userProgress.User = this;
        }

        public void AddCourseProgress(CourseProgress courseProgress)
        {
            CourseProgresses.Add(courseProgress);
            courseProgress.User = this;
        }

        public void AddTopicProgress(TopicProgress topicProgress)
        {
            TopicProgresses.Add(topicProgress);
            topicProgress.User = this;
        }

        public void AddFormSubmission(FormSubmission formSubmission)
        {
            FormSubmissions.Add(formSubmission);
            formSubmission.User = this;
        }
    }
}
