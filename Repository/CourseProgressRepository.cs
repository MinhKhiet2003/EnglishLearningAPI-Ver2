using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{
    public class CourseProgressRepository : ICourseProgressRepository
    {
        private readonly AppDbContext _context;

        public CourseProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        // Tìm CourseProgress theo User và Course
        public async Task<CourseProgress> FindByUserAndCourseAsync(User user, Course course)
        {
            return await _context.CourseProgresses
                .FirstOrDefaultAsync(cp => cp.User.Id == user.Id && cp.Course.Id == course.Id);
        }

        // Lấy tất cả CourseProgress theo User
        public async Task<List<CourseProgress>> GetAllCourseProgressForUserAsync(User user)
        {
            return await _context.CourseProgresses
                .Where(cp => cp.UserId == user.Id)
                .ToListAsync();
        }

        // Lấy danh sách Top 10 khóa học phổ biến nhất
        public async Task<List<(string CourseName, long Count)>> FindTop10PopularCoursesAsync()
        {
            return await _context.CourseProgresses
                .Where(cp => cp.IsCompleted == 1)
                .GroupBy(cp => cp.Course.CourseName)
                .Select(g => new ValueTuple<string, long>(g.Key, g.LongCount()))
                .OrderByDescending(g => g.Item2)
                .Take(10)
                .ToListAsync();
        }

        public async Task SaveAsync(CourseProgress courseProgress)
        {
            _context.CourseProgresses.Add(courseProgress);
            await _context.SaveChangesAsync();
        }
    }
}
