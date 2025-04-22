using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface IUserProgressRepository
    {
        Task<List<UserProgress>> FindAllVocabForUserAsync(User user);
        Task<List<(int Level, long Count)>> CountLevelsByUserAsync(User user);
        Task<List<UserProgress>> FindAllVocabForUserWithExamAsync(int userId);
        Task<UserProgress> GetUserProgressAsync(User user, Vocabulary vocab);
        Task<List<UserProgress>> FindUserProgressByLevelAsync(string search, int level, User user);
        Task<bool> ExistsByUserAndVocabularyAsync(User user, Vocabulary vocabulary);
        Task<List<(string Word, long Count)>> FindTop10PopularVocabsAsync();
        Task<UserProgress> GetUserProgressByIdAsync(User user, int id);

        Task<List<UserProgress>> SaveAllAsync(List<UserProgress> userProgressList);
        Task UpdateAsync(UserProgress userProgress);
        Task DeleteAsync(UserProgress userProgress);
        Task<List<UserProgress>> GetAllAsync();
    }
}
