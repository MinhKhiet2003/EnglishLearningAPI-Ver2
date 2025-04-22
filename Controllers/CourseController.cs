using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Response;
using EnglishLearningAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace EnglishLearningAPI.Controllers
{
    
    [ApiController]
    [Route("api/course")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromForm] string courseName, [FromForm] string description, [FromForm] string courseTarget)
        {
            if (string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(courseTarget))
            {
                return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
            }
            if (_courseService.ExistsByCourseNameAsync(courseName).Result)
            {
                return BadRequest(ApiResponse<object>.error(400, "Đã tồn tại tên khóa học này", "Bad Request"));
            }

            var courseDTO = new CourseDTO
            {
                CourseName = courseName,
                Description = description,
                CourseTarget = courseTarget
            };

            var course = _courseService.CreateCourseAsync(courseDTO).Result;
            return StatusCode(201, ApiResponse<object>.Success(201, "", course));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            try
            {
                var course = _courseService.GetCourseAsync(id).Result;
                return Ok(ApiResponse<object>.Success(200, "", course));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCourse()
        {
            var courses = _courseService.GetAllCoursesAsync().Result;
            if (!courses.Any())
            {
                return BadRequest(ApiResponse<object>.error(400, "Không có khóa học nào!", "Bad Request"));
            }
            return Ok(ApiResponse<object>.Success(200, "", courses));
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetAllCourseForUser([FromQuery] string? courseName, [FromQuery] string? description, [FromQuery] string? courseTarget, [FromQuery] string? sort)
        {
            var courses = await _courseService.GetAllWithTopicsAndVocabsOrderedByCourseNameAsync(courseName??"", description??"", courseTarget??"", sort??"");
            if (!courses.Any())
            {
                return BadRequest(ApiResponse<object>.error(400, "Không có khóa học nào!", "Bad Request"));
            }
            return Ok(ApiResponse<object>.Success(200, "", courses));
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetCourses([FromQuery] int page = 0, [FromQuery] int size = 1, [FromQuery] string courseName = null, [FromQuery] string description = null, [FromQuery] string courseTarget = null, [FromQuery] string sort = null)
        {
            if (size < 1)
            {
                return BadRequest(ApiResponse<object>.error(400, "size phải lớn hơn 0", "Bad Request"));
            }
            var courses = await _courseService.GetCoursesAsync(page, size, courseName, description, courseTarget, sort);
            return Ok(ApiResponse<object>.Success(200, "", courses));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] string courseName, [FromForm] string description, [FromForm] string courseTarget)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(courseTarget))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }
                if (!_courseService.PreUpdateCourseAsync(id, courseName).Result && _courseService.ExistsByCourseNameAsync(courseName).Result)
                {
                    return BadRequest(ApiResponse<object>.error(400, "Đã tồn tại tên khóa học này", "Bad Request"));
                }

                var courseDTO = new CourseDTO
                {
                    CourseName = courseName,
                    Description = description,
                    CourseTarget = courseTarget
                };

                var course = _courseService.UpdateCourseAsync(id, courseDTO).Result;
                return StatusCode(201, ApiResponse<object>.Success(201, "", course));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                _courseService.DeleteCourseAsync(id).Wait();
                return Ok(ApiResponse<object>.Success(200, $"Xóa thành công khóa học với id: {id}", null));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpPost("list")]
        public async Task<IActionResult> CreateList([FromForm]List<ListCourse> list)
        {
            var c = new ImportFromJson();
            var uniqueCourses = RemoveDuplicateCourseNames(list);
            var existingCourseNames = _courseService.CheckExistingCourseNamesAsync(uniqueCourses).Result;
            if (existingCourseNames.Any())
            {
                c.CountError = existingCourseNames.Count;
                c.CountSuccess = uniqueCourses.Count - existingCourseNames.Count;
                c.Error = existingCourseNames;
                return BadRequest(ApiResponse<object>.Success(400, "Đã tồi tại tên", c));
            }
            else
            {
                c.CountError = 0;
                c.CountSuccess = uniqueCourses.Count;
                _courseService.SaveAllAsync(uniqueCourses).Wait();
                return StatusCode(201, ApiResponse<object>.Success(201, "Tạo thành công danh sách course", c));
            }
        }

        private List<ListCourse> RemoveDuplicateCourseNames(List<ListCourse> list)
        {
            var seenCourseNames = new HashSet<string>();
            return list.Where(listCourse => seenCourseNames.Add(listCourse.CourseName)).ToList();
        }
    }
}
