namespace EnglishLearningAPI.DTO
{
    public class ListVocab
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public int TopicId { get; set; }
        public string ExampleSentence { get; set; }
        public string Pronunciation { get; set; }
        public string Audio { get; set; }

        // Constructor không tham số
        public ListVocab() { }

        // Constructor có tham số
        public ListVocab(string word, string meaning, int topicId, string exampleSentence, string pronunciation, string audio)
        {
            Word = word;
            Meaning = meaning;
            TopicId = topicId;
            ExampleSentence = exampleSentence;
            Pronunciation = pronunciation;
            Audio = audio;
        }
    }
}
