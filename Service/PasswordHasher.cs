using BCrypt.Net;
using EnglishLearningAPI.Service.IService;

public class PasswordHasher : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password,GenerateSalt());
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public string GenerateSalt()
    {
        return BCrypt.Net.BCrypt.GenerateSalt(12);
    }
}
