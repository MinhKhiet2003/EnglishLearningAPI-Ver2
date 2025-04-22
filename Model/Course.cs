using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EnglishLearningAPI.Models
{
    [Table("course")]
    public class Course
    {
        [Key]
        [Column("course_id")]
        public int Id { get; set; }

        [Column("course_name")]
        [Required]
        public string CourseName { get; set; }

        [Column("description")]
        [Required]
        public string Description { get; set; }

        [Column("course_target")]
        public string CourseTarget { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual ICollection<Topic> Topics { get; set; }

        [JsonIgnore]
        public virtual ICollection<CourseProgress> CourseProgresses { get; set; }

        public Course()
        {
        }

        public Course(string courseName, string description, string courseTarget)
        {
            CourseName = courseName;
            Description = description;
            CourseTarget = courseTarget;
        }

        public void AddTopic(Topic topic)
        {
            if (Topics == null)
            {
                Topics = new List<Topic>();
            }
            Topics.Add(topic);
            topic.Course = this;
        }

        public void AddCourseProgress(CourseProgress courseProgress)
        {
            if (CourseProgresses == null)
            {
                CourseProgresses = new List<CourseProgress>();
            }
            CourseProgresses.Add(courseProgress);
            courseProgress.Course = this;
        }
    }
}
