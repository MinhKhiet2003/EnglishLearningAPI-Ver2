using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service
{
    public interface IUserProgressService
    {
        Task<List<UserProgress>> GetAllVocabForUserAsync(User user);
        Task<List<UserProgress>> SaveAllVocabForUserAsync(User user, List<Vocabulary> vocabs);
        Task<Dictionary<int, long>> CountLevelsByUserAsync(User user);
        Task<List<UserProgress>> GetAllVocabForUserWithExamAsync(User user);
        Task<UserProgress> GetUserProgressAsync(User user, Vocabulary vocabulary);
        Task<UserProgress> UpdateUserProgressAsync(UserProgress userProgress, int status);
        Task<bool> IsVocabExistForUserAsync(User user, Vocabulary vocab);
        Task<bool> AllVocabulariesAssignedToUserAsync(User user, List<Vocabulary> vocabularies);
        Task DeleteUserProgressAsync(User user, int id);
        Task<Dictionary<string, long>> GetTop10PopularVocabsAsync();
        Task<List<UserProgress>> GetUserProgressByLevelAsync(string search, int level, User user);
        Task ScheduledUserProgressCleanupAsync();
    }
}
