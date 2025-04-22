using System;

namespace EnglishLearningAPI.Models
{
    public class Otp
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }

        // Constructor không tham số
        public Otp() { }

        // Constructor có tham số
        public Otp(string email, string code, DateTime expiryTime)
        {
            Email = email;
            Code = code;
            ExpiryTime = expiryTime;
        }

        // Kiểm tra OTP đã hết hạn chưa
        public bool IsExpired()
        {
            return DateTime.Now > ExpiryTime;
        }
    }
}
