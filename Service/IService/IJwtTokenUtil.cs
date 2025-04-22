using EnglishLearningAPI.Models;
using System.Security.Claims;

namespace EnglishLearningAPI.Service.IService
{
    public interface IJwtTokenUtil
    {
        string GenerateToken(User user);
        string GetEmailFromToken(string token);
        bool ValidateToken(string token, UserDetails userDetails);
        ClaimsPrincipal GetClaimsFromToken(string token);
    }

}
