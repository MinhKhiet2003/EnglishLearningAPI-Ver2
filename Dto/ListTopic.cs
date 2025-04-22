namespace EnglishLearningAPI.DTO
{
    public class ListTopic
    {
        public string TopicName { get; set; }
        public string Description { get; set; }
        public int CourseId { get; set; }

        // Constructor không tham số
        public ListTopic() { }

        // Constructor có tham số
        public ListTopic(string topicName, string description, int courseId)
        {
            TopicName = topicName;
            Description = description;
            CourseId = courseId;
        }
    }
}
