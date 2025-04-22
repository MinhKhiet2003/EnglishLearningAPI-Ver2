using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Response;
using EnglishLearningAPI.Service;
using EnglishLearningAPI.Dto;
using Microsoft.AspNetCore.Authorization;

namespace EnglishLearningAPI.Controllers
{
    
    [ApiController]
    [Route("api/topics")]
    [Route("api/[controller]")]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly ICourseService _courseService;

        public TopicController(ITopicService topicService, ICourseService courseService)
        {
            _topicService = topicService;
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTopic([FromForm] CreateTopicDTO dto)
        {
            var topicName = dto.TopicName;
            var description = dto.Description;
            var courseId = dto.CourseId;
            if (string.IsNullOrEmpty(topicName) || string.IsNullOrEmpty(description))
            {
                return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
            }

            if (await _topicService.ExistsByTopicNameAsync(topicName))
            {
                return BadRequest(ApiResponse<object>.error(400, "Đã tồn tại tên chủ đề tiếng anh này", "Bad Request"));
            }

            var topicDTO = new TopicDTO { TopicName = topicName, Description = description };
            try
            {
                var topic = await _topicService.CreateTopicAsync(topicDTO, courseId);
                return StatusCode(201, ApiResponse<object>.Success(201, "", topic));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopic(int id)
        {
            try
            {
                var topic = await _topicService.GetTopicAsync(id);
                return Ok(ApiResponse<object>.Success(200, "", topic));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTopic()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            if (!topics.Any())
            {
                return BadRequest(ApiResponse<object>.error(400, "Không có chủ đề nào!", "Bad Request"));
            }
            return Ok(ApiResponse<object>.Success(200, "", topics));
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllTopicForUser([FromQuery] string? topicName, [FromQuery] string? description, [FromQuery] int id = 0, [FromQuery] string sort = null)
        {
            var topics = await _topicService.GetAllWithVocabsAsync(topicName??"", description??"", id, sort);
            if (!topics.Any())
            {
                return BadRequest(ApiResponse<object>.error(400, "Không có chủ đề nào!", "Bad Request"));
            }
            return Ok(ApiResponse<object>.Success(200, "", topics));
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetTopics([FromQuery] int page = 0, [FromQuery] int size = 1, [FromQuery] string topicName = null, [FromQuery] string description = null, [FromQuery] int id = 0, [FromQuery] string sort = null)
        {
            if (size < 1)
            {
                return BadRequest(ApiResponse<object>.error(400, "Size phải lớn hơn 0", "Bad Request"));
            }

            var topics = await _topicService.GetTopicsAsync(page, size, topicName, description, id, sort);
            return Ok(ApiResponse<object>.Success(200, "", topics));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTopic(int id, [FromForm] string topicName, [FromForm] string description, [FromForm] int courseId = 0)
        {
            try
            {
                if (string.IsNullOrEmpty(topicName) || string.IsNullOrEmpty(description))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }

                if (!await _topicService.PreUpdateTopicAsync(id, topicName) && await _topicService.ExistsByTopicNameAsync(topicName))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Đã tồn tại tên chủ đề tiếng anh này", "Bad Request"));
                }

                var topicDTO = new TopicDTO { TopicName = topicName, Description = description };
                var topic = await _topicService.UpdateTopicAsync(id, topicDTO, courseId);
                return StatusCode(201, ApiResponse<object>.Success(201, "", topic));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpPost("image/{id}")]
        public async Task<IActionResult> UploadImageTopic(int id, IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }

                if (!IsImageFile(image.ContentType))
                {
                    return BadRequest(ApiResponse<object>.error(400, "File không phải là ảnh", "Bad Request"));
                }

                var topicDTO = new TopicDTO { Image = image };
                var topic = await _topicService.UploadImageTopicAsync(id, topicDTO);
                return StatusCode(201, ApiResponse<object>.Success(201, "", topic));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpDelete("image/{id}")]
        public async Task<IActionResult> DeleteImageTopic(int id)
        {
            try
            {
                await _topicService.DeleteImageTopicAsync(id);
                return Ok(ApiResponse<object>.Success(200, "Xóa thành công ảnh chủ đề với id: " + id, null));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                await _topicService.DeleteTopicAsync(id);
                return Ok(ApiResponse<object>.Success(200, "Xóa thành công chủ đề với id: " + id, null));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> CreateList([FromForm] List<ListTopic> list)
        {
            var uniqueTopics = RemoveDuplicateTopicNames(list);

            var existingTopicNames = await _topicService.CheckExistingTopicNamesAsync(uniqueTopics);
            if (existingTopicNames.Any())
            {
                return BadRequest(ApiResponse<object>.Success(400, "Đã tồn tại tên", new { CountError = existingTopicNames.Count, CountSuccess = uniqueTopics.Count - existingTopicNames.Count, Errors = existingTopicNames }));
            }

            var nonExistingCourseIds = await _courseService.CheckNonExistingIdCoursesAsync(uniqueTopics);
            if (nonExistingCourseIds.Any())
            {
                return BadRequest(ApiResponse<object>.Success(400, "Không tồn tại course với id", new { CountError = nonExistingCourseIds.Count, CountSuccess = uniqueTopics.Count - nonExistingCourseIds.Count, Errors = nonExistingCourseIds }));
            }

            await _topicService.SaveAllAsync(uniqueTopics);
            return StatusCode(201, ApiResponse<object>.Success(201, "Tạo thành công danh sách topic", new { CountError = 0, CountSuccess = uniqueTopics.Count }));
        }

        private List<ListTopic> RemoveDuplicateTopicNames(List<ListTopic> list)
        {
            var seenTopicNames = new HashSet<string>();
            return list.Where(topic => seenTopicNames.Add(topic.TopicName)).ToList();
        }

        private bool IsImageFile(string contentType)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp" };
            return allowedTypes.Contains(contentType);
        }
    }
}
