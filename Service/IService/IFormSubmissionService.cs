using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service.IService
{
    public interface IFormSubmissionService
    {
        Task<FormSubmission> CreateFormSubmissionAsync(FormDTO formDTO, User user);
        Task<FormSubmission> GetFormSubmissionAsync(int id);
        Task<PagedResult<FormSubmission>> GetFormAsync(int page, int size, string email, int type, int status, string sort);
        Task<List<FormSubmission>> GetAllFormSubmissionAsync();
        Task<FormSubmission> UpdateFormSubmissionAsync(int id);
        Task<FormSubmission> RejectedFormSubmissionAsync(int id);
        Task DeleteFormSubmissionAsync(int id);
    }
}
