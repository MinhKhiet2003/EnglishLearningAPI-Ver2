using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{

    public class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course> GetCourseWithTopicAsync(int id)
        {
            return await _context.Set<Course>()
                .Include(c => c.Topics)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsByCourseNameAsync(string courseName)
        {
            return await _context.Set<Course>().AnyAsync(c => c.CourseName == courseName);
        }

        public async Task<List<Course>> FindAllCourseWithTopicAsync()
        {
            return await _context.Set<Course>()
                .Include(c => c.Topics)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Course> AddAsync(Course course)
        {
            await _context.Set<Course>().AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Set<Course>().Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var course = await GetByIdAsync(id);
            if (course != null)
            {
                _context.Set<Course>().Remove(course);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _context.Set<Course>().ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Set<Course>().FindAsync(id);
        }

        public IQueryable<Course> Query()
        {
            return _context.Set<Course>().AsQueryable();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Set<Course>().AnyAsync(c => c.Id == id);
        }
        public async Task SaveAllAsync(List<Course> courses)
        {
            await _context.Set<Course>().AddRangeAsync(courses);
            await _context.SaveChangesAsync();
        }
    }
}
