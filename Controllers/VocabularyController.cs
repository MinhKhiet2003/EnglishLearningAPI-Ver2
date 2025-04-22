using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Response;
using EnglishLearningAPI.Service;
using Microsoft.AspNetCore.Http;
using EnglishLearningAPI.Dto;
using Microsoft.AspNetCore.Authorization;

namespace EnglishLearningAPI.Controllers
{
    
    [ApiController]
    [Route("api/vocabs")]
    public class VocabularyController : ControllerBase
    {
        private readonly IVocabularyService _vocabularyService;
        private readonly IExternalApiService _apiService;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ITopicService _topicService;

        public VocabularyController(IVocabularyService vocabularyService, IExternalApiService apiService, IFirebaseStorageService firebaseStorageService, ITopicService topicService)
        {
            _vocabularyService = vocabularyService;
            _apiService = apiService;
            _firebaseStorageService = firebaseStorageService;
            _topicService = topicService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateVocab([FromForm] CreateVocabDTO dto)
        {
            var word = dto.Word;
            var meaning = dto.Meaning;
            var topicId = dto.TopicId;
            if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(meaning))
            {
                return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
            }

            if (await _vocabularyService.ExistsByWordAsync(word))
            {
                return BadRequest(ApiResponse<object>.error(400, "Đã tồn tại từ vựng này", "Bad Request"));
            }

            try
            {
                var map = await _apiService.GetWordDefinitionAsMapAsync(word);
                var pronunciationList = (List<Dictionary<string, object>>)map["pronunciation"];
                var definitionList = (List<Dictionary<string, object>>)map["definition"];
                var vocabDTO = new VocabDTO
                {
                    Word = word,
                    Meaning = meaning,
                    ExampleSentence = (string)definitionList.First()["text"],
                    Pronunciation = (string)pronunciationList.First()["pron"],
                    Audio = (string)pronunciationList.First()["url"]
                };
                var vocabulary = await _vocabularyService.CreateVocabAsync(vocabDTO, topicId);
                return Created("", ApiResponse<Vocabulary>.Success(201, "", vocabulary));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVocab()
        {
            var vocabularies = await _vocabularyService.GetAllVocabsAsync(null, null, 0, null);
            if (!vocabularies.Any())
            {
                return BadRequest(ApiResponse<object>.error(400, "Không có từ vựng nào!", "Bad Request"));
            }
            return Ok(ApiResponse<List<Vocabulary>>.Success(200, "", vocabularies));
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllVocabForUser([FromQuery] string word = null, [FromQuery] string meaning = null, [FromQuery] int id = 0, [FromQuery] string sort = null)
        {
            var vocabularies = await _vocabularyService.GetAllVocabsAsync(word, meaning, id, sort);
            if (!vocabularies.Any())
            {
                return BadRequest(ApiResponse<object>.error(400, "Không có từ vựng nào!", "Bad Request"));
            }
            return Ok(ApiResponse<List<Vocabulary>>.Success(200, "", vocabularies));
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetVocabs([FromQuery] int page = 0, [FromQuery] int size = 1, [FromQuery] string word = null, [FromQuery] string meaning = null, [FromQuery] int id = 0, [FromQuery] string sort = null)
        {
            if (size < 1)
            {
                return BadRequest(ApiResponse<object>.error(400, "size phải lớn hơn 0", "Bad Request"));
            }
            var pagedResult = await _vocabularyService.GetVocabsAsync(page, size, word, meaning, id, sort);
            return Ok(ApiResponse<PagedResult<Vocabulary>>.Success(200, "", pagedResult));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVocab(int id)
        {
            try
            {
                var vocabulary = await _vocabularyService.GetVocabAsync(id);
                return Ok(ApiResponse<Vocabulary>.Success(200, "", vocabulary));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVocab([FromForm] UpdateVocabDTO dto,int id)
        {
            try
            {
                var word = dto.Word;
                var meaning = dto.Meaning;
                var topicId = dto.TopicId;
                if (string.IsNullOrEmpty(word) || string.IsNullOrEmpty(meaning))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }

                if (!await _vocabularyService.PreUpdateVocabAsync(id, word))
                {
                    if (await _vocabularyService.ExistsByWordAsync(word))
                    {
                        return BadRequest(ApiResponse<object>.error(400, "Đã tồn tại từ vựng này", "Bad Request"));
                    }
                }

                var map = await _apiService.GetWordDefinitionAsMapAsync(word);
                var pronunciationList = (List<Dictionary<string, object>>)map["pronunciation"];
                var definitionList = (List<Dictionary<string, object>>)map["definition"];
                var vocabDTO = new VocabDTO
                {
                    Word = word,
                    Meaning = meaning,
                    ExampleSentence = (string)definitionList.First()["text"],
                    Pronunciation = (string)pronunciationList.First()["pron"],
                    Audio = (string)pronunciationList.First()["url"]
                };
                var vocabulary = await _vocabularyService.UpdateVocabAsync(id, vocabDTO, topicId);
                return Ok(ApiResponse<Vocabulary>.Success(201, "", vocabulary));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVocab(int id)
        {
            try
            {
                await _vocabularyService.DeleteVocabAsync(id);
                return Ok(ApiResponse<object>.Success(200, $"Xóa thành công từ vựng với id: {id}", null));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> CreateList([FromBody] List<ListVocab> list)
        {
            var v = new ImportFromJson();
            var uniqueVocabs = RemoveDuplicateWords(list);
            var existingWords = await _vocabularyService.CheckExistingWordsAsync(uniqueVocabs);
            if (existingWords.Any())
            {
                v.CountError = existingWords.Count;
                v.CountSuccess = uniqueVocabs.Count - existingWords.Count;
                v.Error = existingWords;
                return BadRequest(ApiResponse<ImportFromJson>.Success(400, "Đã tồi tại tên", v));
            }

            var nonExistingTopicId = await _topicService.CheckNonExistingIdTopicsAsync(uniqueVocabs);
            if (nonExistingTopicId.Any())
            {
                v.CountError = nonExistingTopicId.Count;
                v.CountSuccess = uniqueVocabs.Count - nonExistingTopicId.Count;
                v.Error = nonExistingTopicId;
                return BadRequest(ApiResponse<ImportFromJson>.Success(400, "Không tồn tại topic với id", v));
            }

            var checkValidWords = await _apiService.CheckValidWordsAsync(uniqueVocabs);
            if (checkValidWords.Any())
            {
                v.CountError = checkValidWords.Count;
                v.CountSuccess = uniqueVocabs.Count - checkValidWords.Count;
                v.Error = checkValidWords;
                return BadRequest(ApiResponse<ImportFromJson>.Success(400, "Từ không hợp lệ", v));
            }

            var vocabs = await WordsAsync(uniqueVocabs);
            v.CountError = 0;
            v.CountSuccess = uniqueVocabs.Count;
            await _vocabularyService.SaveAllAsync(vocabs);
            return Created("", ApiResponse<ImportFromJson>.Success(201, "Tạo thành công danh sách vocab", v));
        }

        private List<ListVocab> RemoveDuplicateWords(List<ListVocab> list)
        {
            var seenWords = new HashSet<string>();
            return list.Where(listVocab => seenWords.Add(listVocab.Word)).ToList();
        }

        private async Task<List<ListVocab>> WordsAsync(List<ListVocab> list)
        {
            foreach (var vocab in list)
            {
                var map = await _apiService.GetWordDefinitionAsMapAsync(vocab.Word);
                var pronunciationList = (List<Dictionary<string, object>>)map["pronunciation"];
                var definitionList = (List<Dictionary<string, object>>)map["definition"];
                vocab.ExampleSentence = (string)definitionList.First()["text"];
                vocab.Pronunciation = (string)pronunciationList.First()["pron"];
                vocab.Audio = (string)pronunciationList.First()["url"];
            }
            return list;
        }
    }
}
