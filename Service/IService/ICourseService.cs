using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service
{
    public interface ICourseService
    {

        Task<Course> CreateCourseAsync(CourseDTO courseDTO);
        Task<Course> GetCourseAsync(int id);
        Task<List<Course>> GetAllCoursesAsync();
        Task<List<Course>> GetAllWithTopicsAndVocabsOrderedByCourseNameAsync(string courseName, string description, string courseTarget, string sort);
        Task<PagedResult<Course>> GetCoursesAsync(int page, int size, string courseName, string description, string courseTarget, string sort);
        Task<Course> UpdateCourseAsync(int id, CourseDTO courseDTO);
        Task DeleteCourseAsync(int id);
        Task<bool> PreUpdateCourseAsync(int id, string courseName);
        Task<bool> ExistsByCourseNameAsync(string courseName);
        Task<List<string>> CheckNonExistingIdCoursesAsync(List<ListTopic> list);
        Task SaveAllAsync(List<ListCourse> list);
        Task<List<Course>> GetAllCourseWithTopicAsync();
        Task<List<string>> CheckExistingCourseNamesAsync(List<ListCourse> courses);
    }
}