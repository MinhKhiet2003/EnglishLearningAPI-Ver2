namespace EnglishLearningAPI.Dto
{
    public class CreateVocabDTO
    {
        public string Word { get; set; }
        public string Meaning { get; set; }
        public int TopicId { get; set; } = 0;
    }
}
