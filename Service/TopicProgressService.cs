using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;

namespace EnglishLearningAPI.Service
{
    public class TopicProgressService : ITopicProgressService
    {
        private readonly ITopicProgressRepository _topicProgressRepository;

        public TopicProgressService(ITopicProgressRepository topicProgressRepository)
        {
            _topicProgressRepository = topicProgressRepository;
        }

        public async Task CreateTopicProgressIfNotExistAsync(User user, Topic topic, int isCompleted, DateTime? completedAt)
        {
            var existingProgress = await _topicProgressRepository.FindByUserAndTopicAsync(user, topic);

            if (existingProgress == null)
            {
                var newProgress = new TopicProgress
                {
                    User = user,
                    Topic = topic,
                    IsCompleted = isCompleted,
                    CompletedAt = completedAt
                };

                await _topicProgressRepository.SaveAsync(newProgress);
            }
        }

        public async Task<bool> AllTopicAssignedToUserAsync(User user, List<Topic> topics)
        {
            foreach (var topic in topics)
            {
                var topicProgress = await _topicProgressRepository.FindByUserAndTopicAsync(user, topic);
                if (topicProgress == null)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<List<TopicProgress>> GetAllTopicProgressAsync(User user)
        {
            return await _topicProgressRepository.GetAllTopicProgressForUserAsync(user);
        }

        public async Task<Dictionary<string, long>> GetTop10PopularTopicsAsync()
        {
            var results = await _topicProgressRepository.FindTop10PopularTopicsAsync();

            return results
                .OrderByDescending(r => r.Item2)
                .Take(10)
                .ToDictionary(r => r.Item1, r => r.Item2);
        }
    }
}
