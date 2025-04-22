using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly AppDbContext _context;

        public TopicRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy một topic cùng với danh sách từ vựng
        public async Task<Topic> GetTopicWithVocabularyAsync(int id)
        {
            return await _context.Topics
                .Include(t => t.Vocabularies)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // 2. Kiểm tra sự tồn tại của topic dựa trên tên
        public async Task<bool> ExistsByTopicNameAsync(string topicName)
        {
            return await _context.Topics
                .AnyAsync(t => t.TopicName == topicName);
        }

        // 3. Kiểm tra sự tồn tại của topic dựa trên mô tả
        public async Task<bool> ExistsByDescriptionAsync(string description)
        {
            return await _context.Topics
                .AnyAsync(t => t.Description == description);
        }

        // 4. Kiểm tra sự tồn tại của topic dựa trên ID
        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Topics
                .AnyAsync(t => t.Id == id);
        }

        // 5. Lấy tất cả topics với danh sách từ vựng (FETCH JOIN)
        public async Task<List<Topic>> FindAllTopicsWithVocabularyAsync()
        {
            return await _context.Topics
                .Include(t => t.Vocabularies)
                .Include(x => x.TopicProgresses)
                .ToListAsync();
        }

        // 6. Lấy tất cả topics không điều kiện
        public async Task<List<Topic>> FindAllAsync()
        {
            return await _context.Topics.ToListAsync();
        }

        // 7. Lưu một topic
        public async Task<Topic> SaveAsync(Topic topic)
        {
            _context.Topics.Update(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        // 8. Xóa một topic
        public async Task DeleteAsync(Topic topic)
        {
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();
        }

        // 9. Lưu danh sách topics
        public async Task<List<Topic>> SaveAllAsync(List<Topic> topics)
        {
            _context.Topics.AddRange(topics);
            await _context.SaveChangesAsync();
            return topics;
        }

        // 10. Lấy các topic với phân trang
        public async Task<(List<Topic>, int)> FindTopicsPagedAsync(int page, int size, string topicName, string description, int courseId, string sort)
        {
            IQueryable<Topic> query = QueryTopics(topicName, description, courseId, sort);

            int totalItems = await query.CountAsync();
            List<Topic> topics = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            int totalPages = (int)Math.Ceiling(totalItems / (double)size);
            return (topics, totalPages);
        }

        // 11. Tạo query để lọc topic
        public IQueryable<Topic> QueryTopics(string topicName, string description, int id, string sort)
        {
            var query = _context.Topics.Include(x => x.Course).AsQueryable();

            // Lọc theo tên
            if (!string.IsNullOrWhiteSpace(topicName))
            {
                query = query.Where(t => t.TopicName.Contains(topicName));
            }

            // Lọc theo mô tả
            if (!string.IsNullOrWhiteSpace(description))
            {
                query = query.Where(t => t.Description.Contains(description));
            }

            // Lọc theo ID khóa học
            if (id != 0)
            {
                query = query.Where(t => t.CourseId == id);
            }

            // Sắp xếp
            switch (sort)
            {
                case "topicName":
                    query = query.OrderBy(t => t.TopicName);
                    break;
                case "-topicName":
                    query = query.OrderByDescending(t => t.TopicName);
                    break;
                case "description":
                    query = query.OrderBy(t => t.Description);
                    break;
                case "-description":
                    query = query.OrderByDescending(t => t.Description);
                    break;
                case "updatedAt":
                    query = query.OrderBy(t => t.UpdatedAt);
                    break;
                case "-updatedAt":
                    query = query.OrderByDescending(t => t.UpdatedAt);
                    break;
                default:
                    break;
            }

            return query;
        }
        public async Task<bool> PreUpdateTopicAsync(int id, string topicName)
        {
            return await _context.Topics
                .AnyAsync(t => t.Id != id && t.TopicName == topicName);
        }
    }
}
