using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface ITopicRepository
    {
        Task<Topic> GetTopicWithVocabularyAsync(int id);
        Task<bool> ExistsByTopicNameAsync(string topicName);
        Task<bool> ExistsByDescriptionAsync(string description);
        Task<bool> ExistsByIdAsync(int id); 
        Task<List<Topic>> FindAllTopicsWithVocabularyAsync();
        Task<List<Topic>> FindAllAsync(); 
        Task<Topic> SaveAsync(Topic topic); 
        Task DeleteAsync(Topic topic);
        Task<List<Topic>> SaveAllAsync(List<Topic> topics);
        IQueryable<Topic> QueryTopics(string topicName, string description, int id, string sort);
        Task<bool> PreUpdateTopicAsync(int id, string topicName);

    }
}
