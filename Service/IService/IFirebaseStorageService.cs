using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EnglishLearningAPI.Service
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileUrl);
    }
}
