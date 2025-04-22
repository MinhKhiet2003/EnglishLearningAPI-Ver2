using System.Threading.Tasks;
using EnglishLearningAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace EnglishLearningAPI.Service.IService
{
    public interface IUserDetailsService
    {
        Task<UserDetails> LoadUserByUsernameAsync(string username);
        Task<User> FindByEmailAsync(string email);
    }
}