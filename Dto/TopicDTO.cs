using Microsoft.AspNetCore.Http;

namespace EnglishLearningAPI.DTO
{
    public class TopicDTO
    {
        public string TopicName { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
