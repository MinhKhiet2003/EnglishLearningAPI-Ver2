using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface IFormSubmissionRepository
    {
        Task<FormSubmission> GetByIdAsync(int id);
        Task AddAsync(FormSubmission formSubmission);
        void Update(FormSubmission formSubmission);
        void Delete(FormSubmission formSubmission);
        Task SaveChangesAsync();
        IQueryable<FormSubmission> Query();
        Task<List<FormSubmission>> GetAllAsync();
    }
}
