using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;
using EnglishLearningAPI.Service.IService;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Service
{
    public class FormSubmissionService : IFormSubmissionService
    {
        private readonly IFormSubmissionRepository _formSubmissionRepository;

        public FormSubmissionService(IFormSubmissionRepository formSubmissionRepository)
        {
            _formSubmissionRepository = formSubmissionRepository;
        }

        public async Task<FormSubmission> CreateFormSubmissionAsync(FormDTO formDTO, User user)
        {
            var formSubmission = new FormSubmission
            {
                FormType = formDTO.FormType,
                Content = formDTO.Content,
                Status = formDTO.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                User = user
            };

            user.FormSubmissions.Add(formSubmission);
            await _formSubmissionRepository.AddAsync(formSubmission);
            await _formSubmissionRepository.SaveChangesAsync();
            return formSubmission;
        }

        public async Task<FormSubmission> GetFormSubmissionAsync(int id)
        {
            var formSubmission = await _formSubmissionRepository.GetByIdAsync(id);
            if (formSubmission == null)
            {
                throw new Exception($"Không có phiếu nào với id: {id}");
            }
            return formSubmission;
        }

        public async Task<PagedResult<FormSubmission>> GetFormAsync(int page, int size, string email, int type, int status, string sort)
        {
            var query = _formSubmissionRepository.Query();

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(fs => fs.User.Email.Contains(email));
            }

            if (type != -1)
            {
                query = query.Where(fs => fs.FormType == type);
            }

            if (status != -1)
            {
                query = query.Where(fs => fs.Status == status);
            }

            switch (sort)
            {
                case "email":
                    query = query.OrderBy(fs => fs.User.Email);
                    break;
                case "-email":
                    query = query.OrderByDescending(fs => fs.User.Email);
                    break;
                case "updatedAt":
                    query = query.OrderBy(fs => fs.UpdatedAt);
                    break;
                case "-updatedAt":
                    query = query.OrderByDescending(fs => fs.UpdatedAt);
                    break;
                default:
                    break;
            }

            return await PagedResult<FormSubmission>.CreateAsync(query.ToList(), page, size);
        }

        public async Task<List<FormSubmission>> GetAllFormSubmissionAsync()
        {
            return await _formSubmissionRepository.GetAllAsync();
        }

        public async Task<FormSubmission> UpdateFormSubmissionAsync(int id)
        {
            var formSubmission = await _formSubmissionRepository.GetByIdAsync(id);
            if (formSubmission == null)
            {
                throw new Exception($"Không có phiếu nào với id: {id}");
            }

            if (formSubmission.Status < 2)
            {
                formSubmission.Status += 1;
                formSubmission.UpdatedAt = DateTime.UtcNow;
            }

            _formSubmissionRepository.Update(formSubmission);
            await _formSubmissionRepository.SaveChangesAsync();
            return formSubmission;
        }

        public async Task<FormSubmission> RejectedFormSubmissionAsync(int id)
        {
            var formSubmission = await _formSubmissionRepository.GetByIdAsync(id);
            if (formSubmission == null)
            {
                throw new Exception($"Không có phiếu nào với id: {id}");
            }

            formSubmission.Status = 3;
            formSubmission.UpdatedAt = DateTime.UtcNow;

            _formSubmissionRepository.Update(formSubmission);
            await _formSubmissionRepository.SaveChangesAsync();
            return formSubmission;
        }

        public async Task DeleteFormSubmissionAsync(int id)
        {
            var formSubmission = await _formSubmissionRepository.GetByIdAsync(id);
            if (formSubmission == null)
            {
                throw new Exception($"Không có phiếu nào với id: {id}");
            }

            _formSubmissionRepository.Delete(formSubmission);
            await _formSubmissionRepository.SaveChangesAsync();
        }
    }
}