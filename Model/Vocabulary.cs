using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnglishLearningAPI.Models
{
    [Table("vocabulary")]
    public class Vocabulary
    {
        [Key]
        [Column("vocab_id")]
        public int Id { get; set; }

        [Column("word")]
        [Required]
        public string Word { get; set; }

        [Column("meaning")]
        public string Meaning { get; set; }

        [Column("example_sentence")]
        public string ExampleSentence { get; set; }

        [Column("pronunciation")]
        public string Pronunciation { get; set; }

        [Column("audio")]
        public string Audio { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("Topic")]
        [Column("topic_id")]
        public int? TopicId { get; set; }
        public virtual Topic Topic { get; set; }

        public virtual ICollection<UserProgress> UserProgresses { get; set; }

        public Vocabulary()
        {
            UserProgresses = new List<UserProgress>();
        }

        public Vocabulary(string word, string meaning, string exampleSentence, string pronunciation)
        {
            Word = word;
            Meaning = meaning;
            ExampleSentence = exampleSentence;
            Pronunciation = pronunciation;
            UserProgresses = new List<UserProgress>();
        }

    }
}
