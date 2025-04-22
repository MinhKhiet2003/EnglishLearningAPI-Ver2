using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface ITopicProgressRepository
    {
        Task<TopicProgress> FindByUserAndTopicAsync(User user, Topic topic);
        Task SaveAsync(TopicProgress topicProgress);
        Task<List<TopicProgress>> GetAllTopicProgressForUserAsync(User user);
        Task<List<(string TopicName, long Count)>> FindTop10PopularTopicsAsync();
    }
}
