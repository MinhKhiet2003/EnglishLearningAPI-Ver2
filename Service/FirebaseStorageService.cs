using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Service
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public FirebaseStorageService(IConfiguration configuration)
        {
            var firebaseConfigPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseConfigPath);

            _storageClient = StorageClient.Create();
            _bucketName = configuration["Firebase:StorageBucket"];
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{file.FileName}";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var blob = await _storageClient.UploadObjectAsync(_bucketName, fileName, file.ContentType, stream);
                return $"https://storage.googleapis.com/{_bucketName}/{blob.Name}";
            }
        }

        public async Task DeleteFileAsync(string url)
        {
            try
            {
                var uri = new Uri(url);
                var filePath = WebUtility.UrlDecode(uri.AbsolutePath.TrimStart('/'));
                await _storageClient.DeleteObjectAsync(_bucketName, filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file: {ex.Message}", ex);
            }
        }
    }
}
