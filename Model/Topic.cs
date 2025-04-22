using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EnglishLearningAPI.Models
{
    [Table("topic")]
    public class Topic
    {
        [Key]
        [Column("topic_id")]
        public int Id { get; set; }

        [Column("topic_name")]
        public string TopicName { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("image")]
        public string Image { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [ForeignKey("Course")]
        [Column("course_id")]
        public int? CourseId { get; set; }
        public virtual Course Course { get; set; }
        [JsonIgnore]
        public virtual ICollection<Vocabulary> Vocabularies { get; set; }
        [JsonIgnore]
        public virtual ICollection<TopicProgress> TopicProgresses { get; set; }

        public Topic()
        {
            Vocabularies = new List<Vocabulary>();
            TopicProgresses = new List<TopicProgress>();
        }

        public Topic(string topicName, string description) : this()
        {
            TopicName = topicName;
            Description = description;
        }

        public void AddVocabulary(Vocabulary vocabulary)
        {
            if (Vocabularies == null)
            {
                Vocabularies = new List<Vocabulary>();
            }
            Vocabularies.Add(vocabulary);
            vocabulary.Topic = this;
        }

        public void AddTopicProgress(TopicProgress topicProgress)
        {
            if (TopicProgresses == null)
            {
                TopicProgresses = new List<TopicProgress>();
            }
            TopicProgresses.Add(topicProgress);
            topicProgress.Topic = this;
        }
    }
}
