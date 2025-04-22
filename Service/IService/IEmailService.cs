namespace EnglishLearningAPI.Service
{
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email chứa mã OTP đến địa chỉ email người dùng.
        /// </summary>
        /// <param name="to">Địa chỉ email người nhận.</param>
        /// <returns>Mã OTP được tạo và gửi đi.</returns>
        string SendOtpEmail(string to);
    }
}
