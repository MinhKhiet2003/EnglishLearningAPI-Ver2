using System.ComponentModel.DataAnnotations;

namespace EnglishLearningAPI.DTO
{
    public class ListUser
    {
        [Required(ErrorMessage = "Không để trống tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Không để trống email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Không để trống mật khẩu")]
        public string Password { get; set; }

        // Constructor không tham số
        public ListUser() { }

        // Constructor có tham số
        public ListUser(string fullName, string email, string password)
        {
            FullName = fullName;
            Email = email;
            Password = password;
        }
    }
}
