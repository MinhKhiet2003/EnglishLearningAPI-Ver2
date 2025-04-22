using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Response;
using EnglishLearningAPI.Service;
using Microsoft.AspNetCore.Http;
using EnglishLearningAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;

namespace EnglishLearningAPI.Controllers
{
    [ApiController]
    [Route("api/forms")]
    public class FormSubmissionController : ControllerBase
    {
        private readonly IFormSubmissionService _formSubmissionService;
        private readonly IUserService _userService;

        public FormSubmissionController(IFormSubmissionService formSubmissionService, IUserService userService)
        {
            _formSubmissionService = formSubmissionService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateForm([FromHeader(Name = "Authorization")] string authHeader, [FromForm] int formType = -1, [FromForm] string content = "")
        {
            try
            {
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(ApiResponse<object>.error(400, "Invalid Authorization header", "Bad Request"));
                }

                var token = authHeader.Substring(7);
                var user = await _userService.FetchAsync(token);
                if (string.IsNullOrEmpty(content) || formType == -1)
                {
                    return BadRequest(ApiResponse<object>.error(400, "Không để trống dữ liệu", "Bad Request"));
                }
                if (formType < 0 || formType > 6)
                {
                    return BadRequest(ApiResponse<object>.error(400, "Type nằm trong khoảng 0-6", "Bad Request"));
                }

                var formDTO = new FormDTO
                {
                    FormType = formType,
                    Content = content,
                    Status = 0
                };

                var formSubmission = await _formSubmissionService.CreateFormSubmissionAsync(formDTO, user);
                return StatusCode(201, ApiResponse<object>.Success(201, "", formSubmission));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllForms()
        {
            var formSubmissions = await _formSubmissionService.GetAllFormSubmissionAsync();
            return Ok(ApiResponse<object>.Success(200, "", formSubmissions));
        }

        [HttpGet("page")]
        public async Task<IActionResult> GetForms([FromQuery] int page = 0, [FromQuery] int size = 1, [FromQuery] string email = null, [FromQuery] int type = -1, [FromQuery] int status = -1, [FromQuery] string sort = null)
        {
            if (size < 1)
            {
                return BadRequest(ApiResponse<object>.error(400, "size phải lớn hơn 0", "Bad Request"));
            }

            var formSubmissions = await _formSubmissionService.GetFormAsync(page, size, email, type, status, sort);
            return Ok(ApiResponse<object>.Success(200, "", formSubmissions));
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateForm(int id)
        {
            try
            {
                var formSubmission = await _formSubmissionService.UpdateFormSubmissionAsync(id);
                return Ok(ApiResponse<object>.Success(200, "", formSubmission));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpPut("rejected/{id}")]
        public async Task<IActionResult> RejectedForm(int id)
        {
            try
            {
                var formSubmission = await _formSubmissionService.RejectedFormSubmissionAsync(id);
                return Ok(ApiResponse<object>.Success(200, "", formSubmission));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            try
            {
                await _formSubmissionService.DeleteFormSubmissionAsync(id);
                return Ok(ApiResponse<object>.Success(200, $"Xóa thành công form với id: {id}", null));
            }
            catch (Exception e)
            {
                return BadRequest(ApiResponse<object>.error(400, e.Message, "Bad Request"));
            }
        }
    }
}
