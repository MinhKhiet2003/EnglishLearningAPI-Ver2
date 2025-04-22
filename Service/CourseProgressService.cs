using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;

namespace EnglishLearningAPI.Service
{
    public class CourseProgressService : ICourseProgressService
    {
        private readonly ICourseProgressRepository _courseProgressRepository;

        public CourseProgressService(ICourseProgressRepository courseProgressRepository)
        {
            _courseProgressRepository = courseProgressRepository;
        }

        public async Task CreateCourseProgressIfNotExistAsync(User user, Course course, int isCompleted, DateTime? completedAt)
        {
            var existingProgress = await _courseProgressRepository.FindByUserAndCourseAsync(user, course);

            if (existingProgress == null)
            {
                var newProgress = new CourseProgress
                {
                    User = user,
                    Course = course,
                    IsCompleted = isCompleted,
                    CompletedAt = completedAt
                };

                await _courseProgressRepository.SaveAsync(newProgress);
            }
        }

        public async Task<List<CourseProgress>> GetAllCourseProgressAsync(User user)
        {
            return await _courseProgressRepository.GetAllCourseProgressForUserAsync(user);
        }

        public async Task<Dictionary<string, long>> GetTop10PopularCoursesAsync()
        {
            var results = await _courseProgressRepository.FindTop10PopularCoursesAsync();

            return results
                .OrderByDescending(r => r.Item2)
                .Take(10)
                .ToDictionary(r => r.Item1, r => r.Item2);
        }
    }
}
