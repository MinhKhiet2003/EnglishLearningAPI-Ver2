using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface ICourseRepository
    {
        Task<Course> GetCourseWithTopicAsync(int id);
        Task<bool> ExistsByCourseNameAsync(string courseName);
        Task<List<Course>> FindAllCourseWithTopicAsync();
        Task<Course> AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(int id);
        Task<List<Course>> GetAllAsync();
        Task<Course> GetByIdAsync(int id);
        IQueryable<Course> Query();
        Task<bool> ExistsByIdAsync(int id);
        Task SaveAllAsync(List<Course> courses);
    }
}
