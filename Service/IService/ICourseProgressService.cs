using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service
{
    public interface ICourseProgressService
    {
        Task CreateCourseProgressIfNotExistAsync(User user, Course course, int isCompleted, DateTime? completedAt);
        Task<List<CourseProgress>> GetAllCourseProgressAsync(User user);
        Task<Dictionary<string, long>> GetTop10PopularCoursesAsync();
    }
}
