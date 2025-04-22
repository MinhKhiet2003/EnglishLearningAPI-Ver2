using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Response;
using EnglishLearningAPI.Service;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace EnglishLearningAPI.Controllers
{
    [ApiController]
    [Route("api/statistical")]
    public class StatisticalController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserProgressService _userProgressService;
        private readonly ICourseService _courseService;
        private readonly ICourseProgressService _courseProgressService;
        private readonly ITopicService _topicService;
        private readonly ITopicProgressService _topicProgressService;
        private readonly IVocabularyService _vocabularyService;
        private readonly IPaymentService _paymentService;

        public StatisticalController(
            IUserService userService,
            IUserProgressService userProgressService,
            ICourseService courseService,
            ICourseProgressService courseProgressService,
            ITopicService topicService,
            ITopicProgressService topicProgressService,
            IVocabularyService vocabularyService,
            IPaymentService paymentService)
        {
            _userService = userService;
            _userProgressService = userProgressService;
            _courseService = courseService;
            _courseProgressService = courseProgressService;
            _topicService = topicService;
            _topicProgressService = topicProgressService;
            _vocabularyService = vocabularyService;
            _paymentService = paymentService;
        }

        [HttpGet("active_count")]
        public async Task<IActionResult> CountUser()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<object>.Success(200, "", users.Count));
        }

        [HttpGet("new")]
        public async Task<IActionResult> NewUser([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            if (start > end)
            {
                return BadRequest(ApiResponse<object>.error(400, "start lớn hơn end", "Bad Request"));
            }
            var users = await _userService.GetUsersCreatedBetweenAsync(start, end);
            return Ok(ApiResponse<object>.Success(200, "", users.Count));
        }

        [HttpGet("segments")]
        public async Task<IActionResult> Segments()
        {
            var segments = await _userService.GetUserSegmentsAsync();
            return Ok(ApiResponse<object>.Success(200, "", segments));
        }

        [HttpGet("count_course")]
        public async Task<IActionResult> CountCourse()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(ApiResponse<object>.Success(200, "", courses.Count));
        }

        [HttpGet("popular_course")]
        public async Task<IActionResult> PopularCourse()
        {
            var courses = await _courseProgressService.GetTop10PopularCoursesAsync();
            return Ok(ApiResponse<object>.Success(200, "", courses));
        }

        [HttpGet("count_topic")]
        public async Task<IActionResult> CountTopic()
        {
            var topics = await _topicService.GetAllTopicsAsync();
            return Ok(ApiResponse<object>.Success(200, "", topics.Count));
        }

        [HttpGet("popular_topic")]
        public async Task<IActionResult> PopularTopic()
        {
            var topics = await _topicProgressService.GetTop10PopularTopicsAsync();
            return Ok(ApiResponse<object>.Success(200, "", topics));
        }

        [HttpGet("count_vocab")]
        public async Task<IActionResult> CountVocab()
        {
            var vocabularies = await _vocabularyService.GetAllVocabAsync();
            return Ok(ApiResponse<object>.Success(200, "", vocabularies.Count));
        }

        [HttpGet("popular_vocab")]
        public async Task<IActionResult> PopularVocab()
        {
            var vocabularies = await _userProgressService.GetTop10PopularVocabsAsync();
            return Ok(ApiResponse<object>.Success(200, "", vocabularies));
        }

        [HttpGet("expiring_soon")]
        public async Task<IActionResult> ExpiringUser()
        {
            var users = await _userService.GetUsersWithExpiringSubscriptionsAsync();
            return Ok(ApiResponse<object>.Success(200, "", users));
        }

        [HttpGet("course")]
        public async Task<IActionResult> Course()
        {
            var courses = await _courseService.GetAllWithTopicsAndVocabsOrderedByCourseNameAsync("", "", "", "");
            return Ok(ApiResponse<object>.Success(200, "", courses.Count));
        }

        [HttpGet("topic")]
        public async Task<IActionResult> Topic()
        {
            var topics = await _topicService.GetAllWithVocabsAsync("", "", 0, "");
            return Ok(ApiResponse<object>.Success(200, "", topics.Count));
        }

        [HttpGet("sub")]
        public async Task<IActionResult> Sub()
        {
            var sub = await _userService.CountUsersByMonthCurrentYearAsync();
            return Ok(ApiResponse<object>.Success(200, "", sub));
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> Revenue()
        {
            var sum = await _paymentService.GetRevenueAsync();
            return Ok(ApiResponse<object>.Success(200, "", sum));
        }

        private decimal CalculateRevenueByPlan(Dictionary<string, long> userSegments)
        {
            var subscriptionPrices = new Dictionary<string, decimal>
            {
                { "6_months", 399000.0m },
                { "1_year", 699000.0m },
                { "3_years", 1199000.0m }
            };

            return userSegments
                .Where(entry => subscriptionPrices.ContainsKey(entry.Key))
                .Select(entry => subscriptionPrices[entry.Key] * entry.Value)
                .Sum();
        }

        [HttpGet("vip")]
        public async Task<IActionResult> Vip()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<object>.Success(200, "", UsersVip(users).Count));
        }

        private List<User> UsersVip(List<User> users)
        {
            return users.Where(user => !user.SubscriptionPlan.Equals("none")).ToList();
        }
    }
}
