using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<Course> CreateCourseAsync(CourseDTO courseDTO)
        {
            var course = new Course
            {
                CourseName = courseDTO.CourseName,
                Description = courseDTO.Description,
                CourseTarget = courseDTO.CourseTarget,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            return await _courseRepository.AddAsync(course);
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            var course = await _courseRepository.GetCourseWithTopicAsync(id)
                ?? throw new Exception($"Không tồn tại khóa học với id: {id}");

            course.Topics = course.Topics.OrderBy(t => t.CreatedAt).ToList();
            return course;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _courseRepository.GetAllAsync();
        }

        public async Task<List<Course>> GetAllWithTopicsAndVocabsOrderedByCourseNameAsync(string courseName, string description, string courseTarget, string sort)
        {
            var query = _courseRepository.Query();

            if (!string.IsNullOrWhiteSpace(courseName))
                query = query.Where(c => EF.Functions.Like(c.CourseName, $"%{courseName}%"));

            if (!string.IsNullOrWhiteSpace(description))
                query = query.Where(c => EF.Functions.Like(c.Description, $"%{description}%"));

            if (!string.IsNullOrWhiteSpace(courseTarget))
                query = query.Where(c => EF.Functions.Like(c.CourseTarget, $"%{courseTarget}%"));

            query = ApplySorting(query, sort);

            return await query.Include(c => c.Topics)
                              .ThenInclude(t => t.Vocabularies)
                              .ToListAsync();
        }

        public async Task<PagedResult<Course>> GetCoursesAsync(int page, int size, string courseName, string description, string courseTarget, string sort)
        {
            var query = _courseRepository.Query();

            if (!string.IsNullOrWhiteSpace(courseName))
                query = query.Where(c => c.CourseName.Contains(courseName));

            if (!string.IsNullOrWhiteSpace(description))
                query = query.Where(c => c.Description.Contains(description));

            if (!string.IsNullOrWhiteSpace(courseTarget))
                query = query.Where(c => c.CourseTarget.Contains(courseTarget));

            query = ApplySorting(query, sort);

            return await PagedResult<Course>.CreateAsync(query.ToList(), page, size);
        }

        public async Task<Course> UpdateCourseAsync(int id, CourseDTO courseDTO)
        {
            var course = await _courseRepository.GetByIdAsync(id)
                ?? throw new Exception($"Không tồn tại khóa học với id: {id}");

            course.CourseName = courseDTO.CourseName;
            course.Description = courseDTO.Description;
            course.CourseTarget = courseDTO.CourseTarget;
            course.UpdatedAt = DateTime.Now;

            await _courseRepository.UpdateAsync(course);
            return course;
        }

        public async Task DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id)
                ?? throw new Exception($"Không tồn tại khóa học với id: {id}");

            foreach (var topic in course.Topics)
            {
                topic.Course = null;
            }

            await _courseRepository.DeleteAsync(id);
        }

        public async Task<bool> PreUpdateCourseAsync(int id, string courseName)
        {
            var course = await _courseRepository.GetCourseWithTopicAsync(id)
                ?? throw new Exception($"Không tồn tại khóa học với id: {id}");

            return course.CourseName == courseName;
        }

        public async Task<bool> ExistsByCourseNameAsync(string courseName)
        {
            return await _courseRepository.ExistsByCourseNameAsync(courseName);
        }
        public async Task<List<string>> CheckExistingCourseNamesAsync(List<ListCourse> courses)
        {
            var existingCourseNames = new List<string>();
            foreach (var course in courses)
            {
                if (await _courseRepository.ExistsByCourseNameAsync(course.CourseName))
                {
                    existingCourseNames.Add(course.CourseName);
                }
            }
            return existingCourseNames;
        }
        public async Task<List<string>> CheckNonExistingIdCoursesAsync(List<ListTopic> list)
        {
            var nonExisting = new List<string>();

            foreach (var item in list)
            {
                if (!await _courseRepository.ExistsByIdAsync(item.CourseId))
                    nonExisting.Add(item.CourseId.ToString());
            }

            return nonExisting;
        }

        public async Task SaveAllAsync(List<ListCourse> list)
        {
            var courses = list.Select(ConvertToEntity).ToList();
            await _courseRepository.SaveAllAsync(courses);
        }

        private Course ConvertToEntity(ListCourse listCourse)
        {
            return new Course
            {
                CourseName = listCourse.CourseName,
                Description = listCourse.Description,
                CourseTarget = listCourse.CourseTarget,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
        }

        public async Task<List<Course>> GetAllCourseWithTopicAsync()
        {
            return await _courseRepository.FindAllCourseWithTopicAsync();
        }

        private IQueryable<Course> ApplySorting(IQueryable<Course> query, string sort)
        {
            if (!string.IsNullOrWhiteSpace(sort))
            {
                switch (sort)
                {
                    case "courseName":
                        query = query.OrderBy(c => c.CourseName);
                        break;
                    case "-courseName":
                        query = query.OrderByDescending(c => c.CourseName);
                        break;
                    case "description":
                        query = query.OrderBy(c => c.Description);
                        break;
                    case "-description":
                        query = query.OrderByDescending(c => c.Description);
                        break;
                    case "courseTarget":
                        query = query.OrderBy(c => c.CourseTarget);
                        break;
                    case "-courseTarget":
                        query = query.OrderByDescending(c => c.CourseTarget);
                        break;
                    case "updatedAt":
                        query = query.OrderBy(c => c.UpdatedAt);
                        break;
                    case "-updatedAt":
                        query = query.OrderByDescending(c => c.UpdatedAt);
                        break;
                    default:
                        query = query.OrderBy(c => c.CourseName);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(c => c.CourseName);
            }

            return query;
        }
    }
}
