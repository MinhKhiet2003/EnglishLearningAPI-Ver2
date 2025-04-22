namespace EnglishLearningAPI.DTO
{
    public class VocabDTO
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public string ExampleSentence { get; set; }
        public string Pronunciation { get; set; }
        public string Audio { get; set; }

        // Constructor không tham số
        public VocabDTO() { }

        // Constructor có tham số
        public VocabDTO(string word, string meaning, string exampleSentence, string pronunciation, string audio)
        {
            Word = word;
            Meaning = meaning;
            ExampleSentence = exampleSentence;
            Pronunciation = pronunciation;
            Audio = audio;
        }
    }
}
