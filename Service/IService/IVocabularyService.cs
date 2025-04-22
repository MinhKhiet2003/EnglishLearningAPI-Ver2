using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service
{
    public interface IVocabularyService
    {
        Task<Vocabulary> CreateVocabAsync(VocabDTO vocabDTO, int topicId);
        Task<Vocabulary> GetVocabAsync(int id);
        Task<List<Vocabulary>> GetAllVocabsAsync(string word, string meaning, int topicId, string sort);
        Task<PagedResult<Vocabulary>> GetVocabsAsync(int page, int size, string word, string meaning, int topicId, string sort);
        Task<Vocabulary> UpdateVocabAsync(int id, VocabDTO vocabDTO, int topicId);
        Task DeleteVocabAsync(int id);
        Task<bool> ExistsByWordAsync(string word);
        Task<List<string>> CheckExistingIdsAsync(List<int> ids);
        Task SaveAllAsync(List<ListVocab> list);
        Task<List<Vocabulary>> GetTwoRandomVocabsAsync(Vocabulary vocabulary);
        Task<bool> PreUpdateVocabAsync(int id, string word);
        Task<List<string>> CheckExistingWordsAsync(List<ListVocab> list);
        Task<List<Vocabulary>> GetAllVocabAsync();
    }
}
