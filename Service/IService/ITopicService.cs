using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Service
{
    public interface ITopicService
    {
        // Tạo mới một topic
        Task<Topic> CreateTopicAsync(TopicDTO topicDTO, int courseId);

        // Lấy thông tin một topic bằng ID
        Task<Topic> GetTopicAsync(int id);

        // Lấy danh sách tất cả topics
        Task<List<Topic>> GetAllTopicsAsync();

        // Lấy danh sách tất cả topics kèm từ vựng với các bộ lọc
        Task<List<Topic>> GetAllWithVocabsAsync(string topicName, string description, int courseId, string sort);

        // Lấy danh sách topics phân trang với tổng số trang
        Task<PagedResult<Topic>> GetTopicsAsync(int page, int size, string topicName, string description, int courseId, string sort);

        // Cập nhật thông tin một topic
        Task<Topic> UpdateTopicAsync(int id, TopicDTO topicDTO, int courseId);

        // Upload hình ảnh cho topic
        Task<Topic> UploadImageTopicAsync(int id, TopicDTO topicDTO);

        // Xóa hình ảnh của một topic
        Task DeleteImageTopicAsync(int id);

        // Xóa một topic
        Task DeleteTopicAsync(int id);

        // Kiểm tra sự tồn tại của topic dựa trên tên
        Task<bool> ExistsByTopicNameAsync(string topicName);

        // Kiểm tra sự tồn tại của topic dựa trên mô tả
        Task<bool> ExistsByDescriptionAsync(string description);

        // Kiểm tra sự tồn tại của topic dựa trên ID
        Task<bool> ExistsByIdAsync(int id);

        // Kiểm tra các ID không tồn tại trong danh sách
        Task<List<string>> CheckNonExistingIdTopicsAsync(List<ListVocab> list);

        // Kiểm tra các tên topic đã tồn tại trong danh sách
        Task<List<string>> CheckExistingTopicNamesAsync(List<ListTopic> list);

        // Lưu danh sách các topics
        Task SaveAllAsync(List<ListTopic> list);

        // Lấy danh sách tất cả topics kèm theo từ vựng
        Task<List<Topic>> GetAllTopicWithVocabAsync();
        Task<bool> PreUpdateTopicAsync(int id, string topicName);
    }
}
