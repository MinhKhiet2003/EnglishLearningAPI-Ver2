using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;
using EnglishLearningAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Service
{
    public class UserDetailsService : IUserDetailsService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;

        public UserDetailsService(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<UserDetails> LoadUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Where(u => u.Email == username)
                .Select(u => new UserDetails(u.Email, u.Password, u.Role))
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                throw new InvalidOperationException($"Không tìm thấy email {email}");
            }
            return user;
        }
    }
}
