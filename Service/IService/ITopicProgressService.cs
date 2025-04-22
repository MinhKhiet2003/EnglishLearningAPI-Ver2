using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service
{
    public interface ITopicProgressService
    {
        Task CreateTopicProgressIfNotExistAsync(User user, Topic topic, int isCompleted, DateTime? completedAt);
        Task<bool> AllTopicAssignedToUserAsync(User user, List<Topic> topics);
        Task<List<TopicProgress>> GetAllTopicProgressAsync(User user);
        Task<Dictionary<string, long>> GetTop10PopularTopicsAsync();
    }
}
