using EnglishLearningAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Repositories
{
    public interface IVocabularyRepository
    {
        Task<Vocabulary> AddAsync(Vocabulary vocabulary);
        Task<Vocabulary> GetByIdAsync(int id);
        Task<List<Vocabulary>> GetAllAsync();
        Task<Vocabulary> UpdateAsync(Vocabulary vocabulary);
        Task DeleteAsync(Vocabulary vocabulary);
        Task<bool> ExistsByWordAsync(string word);
        Task<bool> ExistsByIdAsync(int id);
        Task<List<int>> GetExistingIdsAsync(List<int> ids);
        Task<List<string>> GetExistingWordsAsync(List<string> words);
        Task AddRangeAsync(List<Vocabulary> vocabularies);
        Task<List<Vocabulary>> FindTwoRandomVocabsAsync(int vocabId, int topicId);
        IQueryable<Vocabulary> Query();
    }
}
