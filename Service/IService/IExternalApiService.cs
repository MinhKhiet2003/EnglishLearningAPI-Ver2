using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;

namespace EnglishLearningAPI.Service
{
    public interface IExternalApiService
    {
        Task<Dictionary<string, object>> GetWordDefinitionAsMapAsync(string word);
        Task<List<string>> CheckValidWordsAsync(List<ListVocab> list);
    }
}
