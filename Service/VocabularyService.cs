using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Service
{
    public class VocabularyService : IVocabularyService
    {
        private readonly IVocabularyRepository _vocabularyRepository;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ITopicService _topicService;

        public VocabularyService(
            IVocabularyRepository vocabularyRepository,
            IFirebaseStorageService firebaseStorageService,
            ITopicService topicService)
        {
            _vocabularyRepository = vocabularyRepository;
            _firebaseStorageService = firebaseStorageService;
            _topicService = topicService;
        }

        public async Task<Vocabulary> CreateVocabAsync(VocabDTO vocabDTO, int topicId)
        {
            var vocabulary = new Vocabulary(vocabDTO.Word, vocabDTO.Meaning, vocabDTO.ExampleSentence, vocabDTO.Pronunciation)
            {
                Audio = vocabDTO.Audio,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (topicId != 0)
            {
                var topic = await _topicService.GetTopicAsync(topicId);
                topic.AddVocabulary(vocabulary);
            }

            return await _vocabularyRepository.AddAsync(vocabulary);
        }

        public async Task<Vocabulary> GetVocabAsync(int id)
        {
            return await _vocabularyRepository.GetByIdAsync(id) ?? throw new Exception($"Vocabulary with id {id} does not exist.");
        }

        public async Task<List<Vocabulary>> GetAllVocabsAsync(string word, string meaning, int topicId, string sort)
        {
            var query = _vocabularyRepository.Query();

            if (!string.IsNullOrEmpty(word))
            {
                query = query.Where(v => EF.Functions.Like(v.Word, $"%{word}%"));
            }

            if (!string.IsNullOrEmpty(meaning))
            {
                query = query.Where(v => EF.Functions.Like(v.Meaning, $"%{meaning}%"));
            }

            if (topicId != 0)
            {
                query = query.Where(v => v.TopicId == topicId);
            }

            query = sort switch
            {
                "word" => query.OrderBy(v => v.Word),
                "-word" => query.OrderByDescending(v => v.Word),
                "meaning" => query.OrderBy(v => v.Meaning),
                "-meaning" => query.OrderByDescending(v => v.Meaning),
                "updatedAt" => query.OrderBy(v => v.UpdatedAt),
                "-updatedAt" => query.OrderByDescending(v => v.UpdatedAt),
                _ => query
            };

            return await query.ToListAsync();
        }

        public async Task<PagedResult<Vocabulary>> GetVocabsAsync(int page, int size, string word, string meaning, int topicId, string sort)
        {
            var query = _vocabularyRepository.Query();

            if (!string.IsNullOrEmpty(word))
            {
                query = query.Where(v => v.Word.Contains(word));
            }

            if (!string.IsNullOrEmpty(meaning))
            {
                query = query.Where(v => meaning.Contains(meaning));
            }

            if (topicId != 0)
            {
                query = query.Where(v => v.TopicId == topicId);
            }

            query = sort switch
            {
                "word" => query.OrderBy(v => v.Word),
                "-word" => query.OrderByDescending(v => v.Word),
                "meaning" => query.OrderBy(v => v.Meaning),
                "-meaning" => query.OrderByDescending(v => v.Meaning),
                "updatedAt" => query.OrderBy(v => v.UpdatedAt),
                "-updatedAt" => query.OrderByDescending(v => v.UpdatedAt),
                _ => query
            };

            return await PagedResult<Vocabulary>.CreateAsync(query.ToList(), page, size);
        }

        public async Task<List<Vocabulary>> GetAllVocabAsync()
        {
            return await _vocabularyRepository.GetAllAsync();
        }

        public async Task<Vocabulary> UpdateVocabAsync(int id, VocabDTO vocabDTO, int topicId)
        {
            var vocabulary = await _vocabularyRepository.GetByIdAsync(id) ?? throw new Exception($"Vocabulary with id {id} does not exist.");

            vocabulary.Word = vocabDTO.Word;
            vocabulary.Meaning = vocabDTO.Meaning;
            vocabulary.ExampleSentence = vocabDTO.ExampleSentence;
            vocabulary.Pronunciation = vocabDTO.Pronunciation;
            vocabulary.Audio = vocabDTO.Audio;
            vocabulary.UpdatedAt = DateTime.UtcNow;

            if (topicId != 0)
            {
                var topic = await _topicService.GetTopicAsync(topicId);
                topic.AddVocabulary(vocabulary);
            }

            return await _vocabularyRepository.UpdateAsync(vocabulary);
        }

        public async Task DeleteVocabAsync(int id)
        {
            var vocabulary = await _vocabularyRepository.GetByIdAsync(id) ?? throw new Exception($"Vocabulary with id {id} does not exist.");
            await _vocabularyRepository.DeleteAsync(vocabulary);
        }

        public async Task<bool> PreUpdateVocabAsync(int id, string word)
        {
            var vocabulary = await _vocabularyRepository.GetByIdAsync(id) ?? throw new Exception($"Vocabulary with id {id} does not exist.");
            return vocabulary.Word.Equals(word, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> ExistsByWordAsync(string word)
        {
            return await _vocabularyRepository.ExistsByWordAsync(word);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _vocabularyRepository.ExistsByIdAsync(id);
        }

        public async Task<List<string>> CheckExistingIdsAsync(List<int> ids)
        {
            var existingIds = await _vocabularyRepository.GetExistingIdsAsync(ids);
            return ids.Except(existingIds).Select(id => id.ToString()).ToList();
        }

        public async Task<List<string>> CheckExistingWordsAsync(List<ListVocab> list)
        {
            var words = list.Select(v => v.Word).ToList();
            var existingWords = await _vocabularyRepository.GetExistingWordsAsync(words);
            return words.Except(existingWords).ToList();
        }

        public async Task SaveAllAsync(List<ListVocab> list)
        {
            var vocabularies = list.Select(ConvertToEntity).ToList();
            await _vocabularyRepository.AddRangeAsync(vocabularies);
        }

        private Vocabulary ConvertToEntity(ListVocab listVocab)
        {
            var vocabulary = new Vocabulary
            {
                Word = listVocab.Word,
                Meaning = listVocab.Meaning,
                ExampleSentence = listVocab.ExampleSentence,
                Pronunciation = listVocab.Pronunciation,
                Audio = listVocab.Audio,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var topic = _topicService.GetTopicAsync(listVocab.TopicId).Result;
            topic.AddVocabulary(vocabulary);

            return vocabulary;
        }

        public async Task<List<Vocabulary>> GetTwoRandomVocabsAsync(Vocabulary vocabulary)
        {
            return await _vocabularyRepository.FindTwoRandomVocabsAsync(vocabulary.Id, vocabulary.TopicId.Value);
        }
    }
}
