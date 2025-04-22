using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EnglishLearningAPI.Dto;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Response;
using EnglishLearningAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EnglishLearningAPI.Controllers
{

    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICourseService _courseService;
        private readonly ITopicService _topicService;
        private readonly IVocabularyService _vocabularyService;
        private readonly IUserProgressService _userProgressService;
        private readonly ICourseProgressService _courseProgressService;
        private readonly ITopicProgressService _topicProgressService;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IEmailService _emailService;

        public UserController(
            IUserService userService,
            ICourseService courseService,
            ITopicService topicService,
            IVocabularyService vocabularyService,
            IUserProgressService userProgressService,
            ICourseProgressService courseProgressService,
            ITopicProgressService topicProgressService,
            IPasswordHasherService passwordHasherService,
            IEmailService emailService)
        {
            _userService = userService;
            _courseService = courseService;
            _topicService = topicService;
            _vocabularyService = vocabularyService;
            _userProgressService = userProgressService;
            _courseProgressService = courseProgressService;
            _topicProgressService = topicProgressService;
            _passwordHasherService = passwordHasherService;
            _emailService = emailService;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm]CreateUserDTO dto)
        {
            var email = dto.Email;
            var password = dto.Password;
            var fullName = dto.FullName;
            var subscriptionPlan = dto.SubscriptionPlan;
            var role = dto.Role;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(subscriptionPlan) ||
                string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
            }

            var usernameRegex = "^[A-Za-zÀ-ỹả-ỹ ]{3,255}$";
            if (!Regex.IsMatch(fullName, usernameRegex))
                return BadRequest(ApiResponse<object>.error(400, "Tên không hợp lệ", "Bad Request"));

            var emailRegex = "^[A-Za-z][A-Za-z0-9_+&*-]*(?:\\.[A-Za-z0-9_+&*-]+)*@" +
                             "(?:[A-Za-z0-9-]+\\.)+[A-Za-z]{2,7}$";
            if (!Regex.IsMatch(email, emailRegex))
                return BadRequest(ApiResponse<object>.error(400, "Email không hợp lệ", "Bad Request"));

            var passwordRegex = "^(?!.*\\s)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$";
            if (!Regex.IsMatch(password, passwordRegex))
                return BadRequest(ApiResponse<object>.error(400,
                    "Mật khẩu phải có ít nhất một chữ hoa, một chữ thường, một chữ số, một ký tự đặc biệt và tối thiểu 8 ký tự và không chứa khoảng trắng",
                    "Bad Request"));

            if (email == password)
                return BadRequest(ApiResponse<object>.error(400, "Email và mật khẩu không được trùng nhau", "Bad Request"));

            if (_userService.ExistsByEmailAsync(email).Result)
                return BadRequest(ApiResponse<object>.error(400, "Email này đã tồn tại", "Bad Request"));

            var salt = _passwordHasherService.GenerateSalt();

            var userDTO = new UserDTO
            {
                Email = email,
                Password = _passwordHasherService.HashPassword(password),
                Salt=salt,
                FullName = fullName,
                SubscriptionPlan = subscriptionPlan,
                Role = role
            };

            var user = _userService.CreateUserAsync(userDTO).Result;
            return StatusCode(201, ApiResponse<object>.Success(201, "", user));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = _userService.GetUserAsync(id).Result;
                return Ok(ApiResponse<object>.Success(200, "", user));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userService.GetAllUsersAsync().Result;
            return Ok(ApiResponse<object>.Success(200, "", users));
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromForm] UpdateUserDTO dto)
        {
            try
            {
                var fullName = dto.FullName;
                var subscriptionPlan = dto.SubscriptionPlan;
                var role = dto.Role;
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(subscriptionPlan) ||
                    string.IsNullOrWhiteSpace(role))
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));

                var usernameRegex = "^[A-Za-zÀ-ỹả-ỹ ]{3,255}$";
                if (!Regex.IsMatch(fullName, usernameRegex))
                    return BadRequest(ApiResponse<object>.error(400, "Tên không hợp lệ", "Bad Request"));

                var userDTO = new UserDTO
                {
                    FullName = fullName,
                    SubscriptionPlan = subscriptionPlan,
                    Role = role
                };

                var updatedUser = _userService.UpdateUserAsync(id, userDTO).Result;
                return Ok(ApiResponse<object>.Success(200, "", updatedUser));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                _userService.DeleteUserAsync(id).Wait();
                return Ok(ApiResponse<object>.Success(200, $"Xóa thành công user với id: {id}", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm]LoginDTO dto)
        {
            try
            {
                var token = await _userService.LoginAsync(dto.Email, dto.Password);
                var user = await _userService.GetUserByEmailAsync(dto.Email);

                if (user == null || string.IsNullOrEmpty(token))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Thông tin đăng nhập không chính xác", "Bad Request"));
                }

                var loginResponse = new LoginResponse(token, user);
                return Ok(ApiResponse<LoginResponse>.Success(200, "Đăng nhập thành công", loginResponse));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, $"An error occurred: {ex.Message}", "Bad Request"));
            }
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO dto)
        {
            var email = dto.Email;
            var password = dto.Password;
            var fullName = dto.FullName;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
            }

            var usernameRegex = "^[A-Za-zÀ-ỹả-ỹ ]{3,255}$";
            if (!Regex.IsMatch(fullName, usernameRegex))
                return BadRequest(ApiResponse<object>.error(400, "Tên không hợp lệ", "Bad Request"));

            var emailRegex = "^[A-Za-z][A-Za-z0-9_+&*-]*(?:\\.[A-Za-z0-9_+&*-]+)*@" +
                             "(?:[A-Za-z0-9-]+\\.)+[A-Za-z]{2,7}$";
            if (!Regex.IsMatch(email, emailRegex))
                return BadRequest(ApiResponse<object>.error(400, "Email không hợp lệ", "Bad Request"));

            var passwordRegex = "^(?!.*\\s)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$";
            if (!Regex.IsMatch(password, passwordRegex))
                return BadRequest(ApiResponse<object>.error(400,
                    "Mật khẩu phải có ít nhất một chữ hoa, một chữ thường, một chữ số, một ký tự đặc biệt và tối thiểu 8 ký tự và không chứa khoảng trắng",
                    "Bad Request"));

            if (email == password)
                return BadRequest(ApiResponse<object>.error(400, "Email và mật khẩu không được trùng nhau", "Bad Request"));

            if (await _userService.ExistsByEmailAsync(email))
                return BadRequest(ApiResponse<object>.error(400, "Email này đã tồn tại", "Bad Request"));

            string salt = _passwordHasherService.GenerateSalt();

            var userDTO = new UserDTO
            {
                Email = email,
                Password = _passwordHasherService.HashPassword(password),
                Salt = salt,
                FullName = fullName,
                SubscriptionPlan = "none",
                Role = "ROLE_USER"
            };

            var user = await _userService.RegisterAsync(userDTO);
            return StatusCode(201, ApiResponse<object>.Success(201, "", user));
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromForm] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(ApiResponse<object>.error(400, "Token không được để trống", "Bad Request"));
            }

            try
            {
                await _userService.LogoutAsync(token);
                return Ok(ApiResponse<object>.Success(200, "Đăng xuất thành công", null));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.error(400, ex.Message, "Bad Request"));
            }
        }
        
        [HttpGet("page")]
        public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 0,
        [FromQuery] int size = 1,
        [FromQuery] string email = null,
        [FromQuery] string fullName = null,
        [FromQuery] string role = null,
        [FromQuery] string subscriptionPlan = null,
        [FromQuery] string sort = null)
        {
            if (size < 1)
            {
                return BadRequest(ApiResponse<object>.error(400, "size phải lớn hơn 0", "Bad Request"));
            }

            var users = await _userService.GetUsersAsync(page, size, email, fullName, role, subscriptionPlan, sort);

            return Ok(ApiResponse<object>.Success(200,"Ok", users));
        }


        [HttpGet("account")]
        public async Task<IActionResult> Fetch()
        {
            string authHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
            }

            string token = authHeader.Substring(7);
            User user = await _userService.FetchAsync(token);

            if (user == null)
            {
                return NotFound(ApiResponse<object>.error(404, "User not found", "Not Found"));
            }

            return Ok(ApiResponse<object>.Success(200, "", user));
        }
        
        [HttpDelete("sub/{id}")]
        public async Task<IActionResult> DeleteSubUser(int id)
        {
            try
            {
                await _userService.DeleteSubscriptionAsync(id);
                return Ok(new ApiResponse<string>
                {
                    StatusCode = 200,
                    Message = $"Successfully deleted subscription for user with id: {id}",
                    Data = null
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = 400,
                    Message = e.Message,
                    Error = "Bad Request"
                });
            }
        }
        
        [HttpPost("list")]
        public async Task<IActionResult> CreateList([FromForm] List<UserDTO> list)
        {
            var u = new ImportFromJson();
            var uniqueUsers = RemoveDuplicateEmails(list);
            var usernameRegex = "^[A-Za-zÀ-ỹả-ỹ ]{3,255}$";
            var name = new List<string>();

            foreach (var dto in uniqueUsers)
            {
                if (!Regex.IsMatch(dto.FullName, usernameRegex))
                {
                    name.Add(dto.FullName);
                }
            }

            if (name.Any())
            {
                u.CountError = name.Count;
                u.CountSuccess = uniqueUsers.Count - name.Count;
                u.Error = name;
                return BadRequest(ApiResponse<object>.Success(400, "Tên không hợp lệ", u));
            }

            var emailRegex = "^[A-Za-z][A-Za-z0-9_+&*-]*(?:\\.[A-Za-z0-9_+&*-]+)*@" +
                             "(?:[A-Za-z0-9-]+\\.)+[A-Za-z]{2,7}$";
            var email = new List<string>();

            foreach (var dto in uniqueUsers)
            {
                if (!Regex.IsMatch(dto.Email, emailRegex))
                {
                    email.Add(dto.Email);
                }
            }

            if (email.Any())
            {
                u.CountError = email.Count;
                u.CountSuccess = uniqueUsers.Count - email.Count;
                u.Error = email;
                return BadRequest(ApiResponse<object>.Success(400, "Email không hợp lệ", u));
            }

            var passwordRegex = "^(?!.*\\s)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$";
            var pass = new List<string>();

            foreach (var dto in uniqueUsers)
            {
                if (!Regex.IsMatch(dto.Password, passwordRegex))
                {
                    pass.Add(dto.Password);
                }
            }

            if (pass.Any())
            {
                u.CountError = pass.Count;
                u.CountSuccess = uniqueUsers.Count - pass.Count;
                u.Error = pass;
                return BadRequest(ApiResponse<object>.Success(400, "Mật khẩu phải có ít nhất một chữ hoa, một chữ thường, một chữ số, một ký tự đặc biệt và tối thiểu 8 ký tự và không chứa khoảng trắng", u));
            }

            var checkEmailAndPass = new List<string>();

            foreach (var dto in uniqueUsers)
            {
                if (dto.Email == dto.Password)
                {
                    checkEmailAndPass.Add(dto.Password);
                }
            }

            if (checkEmailAndPass.Any())
            {
                u.CountError = checkEmailAndPass.Count;
                u.CountSuccess = uniqueUsers.Count - checkEmailAndPass.Count;
                u.Error = checkEmailAndPass;
                return BadRequest(ApiResponse<object>.Success(400, "Email và mật khẩu không được trùng nhau", u));
            }

            var existingEmails = await _userService.CheckExistingEmailsAsync(uniqueUsers);

            if (existingEmails.Any())
            {
                u.CountError = existingEmails.Count;
                u.CountSuccess = uniqueUsers.Count - existingEmails.Count;
                u.Error = existingEmails;
                return BadRequest(ApiResponse<object>.Success(400, "Đã tồn tại email", u));
            }
            else
            {
                u.CountError = 0;
                u.CountSuccess = uniqueUsers.Count;
                await _userService.SaveAllUsersAsync(uniqueUsers);
                return StatusCode(201, ApiResponse<object>.Success(201, "Tạo thành công danh sách người dùng", u));
            }
        }

        private List<UserDTO> RemoveDuplicateEmails(List<UserDTO> list)
        {
            return list.GroupBy(x => x.Email).Select(x => x.First()).ToList();
        }

        [HttpGet("learned_vocabulary")]
        public async Task<IActionResult> GetAllVocabForUser()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                List<Vocabulary> vocabularies = new List<Vocabulary>();

                var userProgressList = await _userProgressService.GetAllVocabForUserAsync(user);
                foreach (var userProgress in userProgressList)
                {
                    vocabularies.Add(userProgress.Vocabulary);
                }

                if (!vocabularies.Any())
                {
                    return BadRequest(ApiResponse<object>.error(400, "Bạn chưa có từ vựng để ôn tập", "Bad Request"));
                }
                else
                {
                    return Ok(ApiResponse<object>.Success(200, "", vocabularies));
                }
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpPost("selected_vocab")]
        public async Task<IActionResult> SaveAllVocabForUser([FromBody] VocabSelected list)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                List<string> existingVocab = await _vocabularyService.CheckExistingIdsAsync(list.Id);
                ImportFromJson v = new ImportFromJson();

                if (existingVocab.Any())
                {
                    v.CountError = existingVocab.Count;
                    v.CountSuccess = list.Id.Count - existingVocab.Count;
                    v.Error = existingVocab;
                    return BadRequest(ApiResponse<object>.Success(400, "Không tồn tại vocab với id", v));
                }

                List<Vocabulary> vocabularies = new List<Vocabulary>();
                List<Vocabulary> learn = new List<Vocabulary>();

                foreach (int id in list.Id)
                {
                    var vocab = await _vocabularyService.GetVocabAsync(id);
                    vocabularies.Add(vocab);
                    learn.Add(vocab);
                }

                foreach (var vo in vocabularies)
                {
                    if (await _userProgressService.IsVocabExistForUserAsync(user, vo))
                    {
                        learn.Remove(vo);
                    }
                }

                if (learn.Any())
                {
                    var us = await _userProgressService.SaveAllVocabForUserAsync(user, learn);

                    foreach (var u in us)
                    {
                        await _topicProgressService.CreateTopicProgressIfNotExistAsync(user, u.Vocabulary.Topic, 1, DateTime.Now);
                    }

                    var courses = await _courseService.GetAllCourseWithTopicAsync();
                    foreach (var course in courses)
                    {
                        var topics = course.Topics.ToList();
                        if (await _topicProgressService.AllTopicAssignedToUserAsync(user, topics))
                        {
                            await _courseProgressService.CreateCourseProgressIfNotExistAsync(user, course, 1, DateTime.Now);
                        }
                    }
                }

                return Ok(ApiResponse<object>.Success(200, "Selected vocabulary saved for review", null));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpGet("review_stats")]
        public async Task<IActionResult> CountLevelsByUser()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                var level = await _userProgressService.CountLevelsByUserAsync(user);

                if (!level.Any())
                {
                    for (int n = 1; n < 6; n++)
                    {
                        level[n] = 0;
                    }
                }

                return Ok(ApiResponse<object>.Success(200, "", level));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpGet("review_vocab")]
        public async Task<IActionResult> ReviewVocabByUser()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                var us = await _userProgressService.GetAllVocabForUserWithExamAsync(user);
                List<ExamResponse> exam = new List<ExamResponse>();

                foreach (var u in us)
                {
                    var incorrect = await _vocabularyService.GetTwoRandomVocabsAsync(u.Vocabulary);
                    ExamResponse ex = new ExamResponse(RandomNumber(), u.Vocabulary, incorrect);
                    exam.Add(ex);
                }

                if (!exam.Any())
                {
                    return BadRequest(ApiResponse<object>.error(400, "Bạn chưa có từ vựng để ôn tập", "Bad Request"));
                }

                return Ok(ApiResponse<object>.Success(200, "", exam));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpPost("complete_review")]
        public async Task<IActionResult> CompleteReview([FromBody] List<VocabReview> reviews)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                List<UserProgress> progresses = new List<UserProgress>();

                foreach (var review in reviews)
                {
                    var vocabulary = await _vocabularyService.GetVocabAsync(review.Id);
                    var us = await _userProgressService.GetUserProgressAsync(user, vocabulary);
                    var u = await _userProgressService.UpdateUserProgressAsync(us, review.Status);
                    progresses.Add(u);
                }

                return Ok(ApiResponse<object>.Success(200, "", progresses));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpGet("wordbook")]
        public async Task<IActionResult> Wordbook([FromQuery] string? search, [FromQuery] int level = 0)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                var us = await _userProgressService.GetUserProgressByLevelAsync(search ?? "", level, user);

                return Ok(ApiResponse<object>.Success(200, "", us));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpDelete("wordbook/{id}")]
        public async Task<IActionResult> DeleteWordbook(int id)
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                await _userProgressService.DeleteUserProgressAsync(user, id);

                return Ok(ApiResponse<object>.Success(200, $"Xoa thanh cong vocab trong workbook voi id {id}", null));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpGet("course_progress")]
        public async Task<IActionResult> CourseProgress()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                var courseProgresses = await _courseProgressService.GetAllCourseProgressAsync(user);

                if (!courseProgresses.Any())
                {
                    return BadRequest(ApiResponse<object>.error(400, "Chưa có khóa học nào hoàn thành", "Bad Request"));
                }

                return Ok(ApiResponse<object>.Success(200, "", courseProgresses));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        
        [HttpGet("topic_progress")]
        public async Task<IActionResult> TopicProgress()
        {
            try
            {
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);
                var topicProgresses = await _topicProgressService.GetAllTopicProgressAsync(user);

                if (!topicProgresses.Any())
                {
                    return BadRequest(ApiResponse<object>.error(400, "Chưa có chủ đề nào hoàn thành", "Bad Request"));
                }

                return Ok(ApiResponse<object>.Success(200, "", topicProgresses));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDTO dto)
        {
            try
            {
                var oldPassword = dto.OldPassword;
                var newPassword = dto.NewPassword;
                string authHeader = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Authorization header is missing or invalid", "Bad Request"));
                }

                string token = authHeader.Substring(7);
                User user = await _userService.FetchAsync(token);

                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }

                string passwordRegex = "^(?!.*\\s)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$";
                if (!Regex.IsMatch(newPassword, passwordRegex))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Mật khẩu phải có ít nhất một chữ hoa, một chữ thường, một chữ số, một ký tự đặc biệt và tối thiểu 8 ký tự và không chứa khoảng trắng", "Bad Request"));
                }

                if (! _passwordHasherService.VerifyPassword(oldPassword, user.Password))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Mật khẩu cũ không chính xác", "Bad Request"));
                }

                if (oldPassword == newPassword)
                {
                    return BadRequest(ApiResponse<object>.error(400, "Mật khẩu mới phải khác mật khẩu cũ", "Bad Request"));
                }

                user.Password = newPassword;
                var updatedUser = await _userService.ChangePasswordAsync(user);

                return Ok(ApiResponse<object>.Success(200, "", updatedUser));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
        [HttpPost("reset_password")]
        public async Task<IActionResult> ResetPassword([FromForm] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }
                if (!await _userService.ExistsByEmailAsync(email))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không tồn tại email này", "Bad Request"));
                }
                string otp = _emailService.SendOtpEmail(email);
                var otpObject = new Otp(email, otp, DateTime.Now.AddMinutes(5));
                return Ok(otpObject);
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpPost("new_password")]
        public async Task<IActionResult> NewPassword([FromForm] NewPasswordDTO dto)
        {
            try
            {
                var email = dto.Email;
                var password = dto.Password;
                if (string.IsNullOrEmpty(password))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }
                string passwordRegex = "^(?!.*\\s)(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%^&*(),.?\":{}|<>]).{8,}$";
                if (!Regex.IsMatch(password, passwordRegex))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Mật khẩu phải có ít nhất một chữ hoa, một chữ thường, một chữ số, một ký tự đặc biệt và tối thiểu 8 ký tự và không chứa khoảng trắng", "Bad Request"));
                }
                var user = _userService.GetUserByEmailAsync(email).Result;
                user.Password = password;
                var updatedUser = await _userService.ChangePasswordAsync(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        private int RandomNumber()
        {
            Random random = new Random();
            return random.Next(1, 7);
        }

    }
}
