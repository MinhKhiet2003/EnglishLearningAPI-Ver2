namespace EnglishLearningAPI.Dto
{
    public class CreateTopicDTO
    {
        public string TopicName { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; } = 0;
    }
}
