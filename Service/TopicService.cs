using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EnglishLearningAPI.Service
{
    public class TopicService : ITopicService
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly ICourseService _courseService;

        public TopicService(ITopicRepository topicRepository, IFirebaseStorageService firebaseStorageService, ICourseService courseService)
        {
            _topicRepository = topicRepository;
            _firebaseStorageService = firebaseStorageService;
            _courseService = courseService;
        }
        public async Task<bool> PreUpdateTopicAsync(int id, string topicName)
        {
            return await _topicRepository.PreUpdateTopicAsync(id, topicName);
        }
        public async Task<PagedResult<Topic>> GetTopicsAsync(int page, int size, string topicName, string description, int id, string sort)
        {
            // Xây dựng Specification để lọc các chủ đề
            var topicsQuery = _topicRepository.QueryTopics(topicName, description, id, sort).ToList();

            // Tổng số bản ghi
            int totalRecords = topicsQuery.Count();
            int totalPages = (int)Math.Ceiling((double)totalRecords / size);

            // Lấy dữ liệu phân trang
            return await PagedResult<Topic>.CreateAsync(topicsQuery, page, size);
        }
        public async Task<Topic> CreateTopicAsync(TopicDTO topicDTO, int courseId)
        {
            var topic = new Topic
            {
                TopicName = topicDTO.TopicName,
                Description = topicDTO.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Image = "",
            };

            if (courseId != 0)
            {
                var course = await _courseService.GetCourseAsync(courseId);
                if(course == null)
                {
                    throw new Exception($"Không tìm thấy khóa học");
                }
                topic.CourseId = course.Id;
            }

            return await _topicRepository.SaveAsync(topic);
        }

        public async Task<Topic> GetTopicAsync(int id)
        {
            var topic = await _topicRepository.GetTopicWithVocabularyAsync(id);
            if (topic == null)
                throw new Exception($"Topic with id {id} does not exist.");

            return topic;
        }

        public async Task<List<Topic>> GetAllTopicsAsync()
        {
            return await _topicRepository.FindAllAsync();
        }

        public async Task<List<Topic>> GetAllWithVocabsAsync(string topicName, string description, int courseId, string sort)
        {
            var topics = await _topicRepository.FindAllTopicsWithVocabularyAsync();

            if (!string.IsNullOrEmpty(topicName))
                topics = topics.Where(t => t.TopicName.Contains(topicName, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(description))
                topics = topics.Where(t => t.Description.Contains(description, StringComparison.OrdinalIgnoreCase)).ToList();

            if (courseId != 0)
                topics = topics.Where(t => t.CourseId == courseId).ToList();

            // Sorting logic
            topics = sort switch
            {
                "topicName" => topics.OrderBy(t => t.TopicName).ToList(),
                "-topicName" => topics.OrderByDescending(t => t.TopicName).ToList(),
                "description" => topics.OrderBy(t => t.Description).ToList(),
                "-description" => topics.OrderByDescending(t => t.Description).ToList(),
                "updatedAt" => topics.OrderBy(t => t.UpdatedAt).ToList(),
                "-updatedAt" => topics.OrderByDescending(t => t.UpdatedAt).ToList(),
                _ => topics
            };

            return topics;
        }

        public async Task<Topic> UpdateTopicAsync(int id, TopicDTO topicDTO, int courseId)
        {
            var topic = await _topicRepository.GetTopicWithVocabularyAsync(id);
            if (topic == null)
                throw new Exception($"Topic with id {id} does not exist.");

            topic.TopicName = topicDTO.TopicName;
            topic.Description = topicDTO.Description;
            topic.UpdatedAt = DateTime.UtcNow;

            if (courseId != 0)
            {
                var course = await _courseService.GetCourseAsync(courseId);
                course.Topics.Add(topic);
            }

            return await _topicRepository.SaveAsync(topic);
        }

        public async Task<Topic> UploadImageTopicAsync(int id, TopicDTO topicDTO)
        {
            var topic = await _topicRepository.GetTopicWithVocabularyAsync(id);
            if (topic == null)
                throw new Exception($"Topic with id {id} does not exist.");

            var url = await _firebaseStorageService.UploadFileAsync(topicDTO.Image);

            if (!string.IsNullOrEmpty(topic.Image))
            {
                await _firebaseStorageService.DeleteFileAsync(topic.Image);
            }

            topic.Image = url;
            return await _topicRepository.SaveAsync(topic);
        }

        public async Task DeleteImageTopicAsync(int id)
        {
            var topic = await _topicRepository.GetTopicWithVocabularyAsync(id);
            if (topic == null)
                throw new Exception($"Topic with id {id} does not exist.");

            if (!string.IsNullOrEmpty(topic.Image))
            {
                await _firebaseStorageService.DeleteFileAsync(topic.Image);
                topic.Image = string.Empty;
                await _topicRepository.SaveAsync(topic);
            }
        }

        public async Task DeleteTopicAsync(int id)
        {
            var topic = await _topicRepository.GetTopicWithVocabularyAsync(id);
            if (topic == null)
                throw new Exception($"Topic with id {id} does not exist.");

            if (!string.IsNullOrEmpty(topic.Image))
            {
                await _firebaseStorageService.DeleteFileAsync(topic.Image);
            }

            foreach (var vocab in topic.Vocabularies)
            {
                vocab.Topic = null;
            }

            await _topicRepository.DeleteAsync(topic);
        }

        public async Task<bool> ExistsByTopicNameAsync(string topicName)
        {
            return await _topicRepository.ExistsByTopicNameAsync(topicName);
        }

        public async Task<bool> ExistsByDescriptionAsync(string description)
        {
            return await _topicRepository.ExistsByDescriptionAsync(description);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _topicRepository.ExistsByIdAsync(id);
        }

        public async Task<List<string>> CheckNonExistingIdTopicsAsync(List<ListVocab> list)
        {
            var nonExistingIds = new List<string>();

            foreach (var vocab in list)
            {
                if (!await ExistsByIdAsync(vocab.TopicId))
                {
                    nonExistingIds.Add(vocab.TopicId.ToString());
                }
            }

            return nonExistingIds;
        }

        public async Task<List<string>> CheckExistingTopicNamesAsync(List<ListTopic> list)
        {
            var existingNames = new List<string>();

            foreach (var topic in list)
            {
                if (await ExistsByTopicNameAsync(topic.TopicName))
                {
                    existingNames.Add(topic.TopicName);
                }
            }

            return existingNames;
        }

        public async Task SaveAllAsync(List<ListTopic> list)
        {
            var topics = list.Select(topic => new Topic
            {
                TopicName = topic.TopicName,
                Description = topic.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CourseId = topic.CourseId
            }).ToList();

            await _topicRepository.SaveAllAsync(topics);
        }

        public async Task<List<Topic>> GetAllTopicWithVocabAsync()
        {
            return await _topicRepository.FindAllTopicsWithVocabularyAsync();
        }
    }
}
