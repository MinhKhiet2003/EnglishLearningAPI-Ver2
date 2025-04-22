using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace EnglishLearningAPI.Config
{
    public class FirebaseConfig
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Đường dẫn đến tệp JSON của Firebase
            var firebaseConfigPath = configuration["Firebase:ConfigPath"];

            if (FirebaseApp.DefaultInstance == null)
            {
                using (var stream = new FileStream(firebaseConfigPath, FileMode.Open, FileAccess.Read))
                {
                    var options = new AppOptions()
                    {
                        Credential = GoogleCredential.FromStream(stream)
                    };
                    FirebaseApp.Create(options);
                }
            }

            // Cấu hình StorageBucket
            var storageBucket = configuration["Firebase:StorageBucket"];
            if (!string.IsNullOrEmpty(storageBucket))
            {
                // Cấu hình StorageBucket nếu cần thiết
                // Ví dụ: FirebaseStorage.DefaultInstance.Bucket = storageBucket;
            }
        }
    }
}
