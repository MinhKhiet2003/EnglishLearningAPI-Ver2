using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface ICourseProgressRepository
    {
        Task<CourseProgress> FindByUserAndCourseAsync(User user, Course course);
        Task SaveAsync(CourseProgress courseProgress);
        Task<List<CourseProgress>> GetAllCourseProgressForUserAsync(User user);
        Task<List<(string CourseName, long Count)>> FindTop10PopularCoursesAsync();
    }
}
