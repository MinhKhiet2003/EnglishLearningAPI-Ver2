using System;
using System.Net;
using System.Net.Mail;
using System.Globalization;

namespace EnglishLearningAPI.Service
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com", // Thay bằng SMTP server của bạn
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential("your-email@gmail.com", "your-password")
            };
        }

        /// <summary>
        /// Gửi email OTP đến người dùng.
        /// </summary>
        /// <param name="to">Địa chỉ email người nhận.</param>
        /// <returns>Chuỗi OTP 6 chữ số.</returns>
        public string SendOtpEmail(string to)
        {
            string otp = GenerateOtp();
            DateTime expirationTime = DateTime.Now.AddMinutes(5);
            string formattedExpirationTime = expirationTime.ToString("HH:mm dd-MM-yyyy", CultureInfo.InvariantCulture);

            // Tạo nội dung email
            string subject = "Easy Vocab: LẤY LẠI MẬT KHẨU";
            string body = $"Mã OTP để bạn lấy lại mật khẩu là: {otp}\nMã sẽ hết hạn vào: {formattedExpirationTime}";

            // Cấu hình message
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@gmail.com", "Easy Vocab"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(to);

            try
            {
                _smtpClient.Send(mailMessage);
                return otp;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gửi email: " + ex.Message);
            }
        }

        /// <summary>
        /// Sinh mã OTP ngẫu nhiên gồm 6 chữ số.
        /// </summary>
        /// <returns>Chuỗi OTP 6 chữ số.</returns>
        private string GenerateOtp()
        {
            Random random = new Random();
            return random.Next(0, 999999).ToString("D6"); // 6 chữ số
        }
    }
}
