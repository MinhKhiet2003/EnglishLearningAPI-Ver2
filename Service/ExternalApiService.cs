using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Service.IService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EnglishLearningAPI.Service
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalApiService> _logger;
        private const string EXTERNAL_API_URL = "https://audio.easyvocab.click/api/dictionary/en/";

        public ExternalApiService(IHttpClientFactory httpClientFactory, ILogger<ExternalApiService> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<Dictionary<string, object>> GetWordDefinitionAsMapAsync(string word)
        {
            var url = $"{EXTERNAL_API_URL}{word}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                if (result != null && result.ContainsKey("error"))
                {
                    throw new Exception($"{word} không hợp lệ");
                }
                return result;
            }
            else
            {
                throw new Exception($"{word} không hợp lệ");
            }
        }

        public async Task<List<string>> CheckValidWordsAsync(List<ListVocab> list)
        {
            var invalidWords = new List<string>();
            foreach (var vocab in list)
            {
                try
                {
                    await GetWordDefinitionAsMapAsync(vocab.Word);
                }
                catch (Exception)
                {
                    invalidWords.Add(vocab.Word);
                }
            }
            return invalidWords;
        }
    }
}
