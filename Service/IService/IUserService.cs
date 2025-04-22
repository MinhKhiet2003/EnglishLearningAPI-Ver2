using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.DTO;
using EnglishLearningAPI.Models;

public interface IUserService
{
    Task<User> CreateUserAsync(UserDTO userDTO);
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserAsync(int id);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> UpdateUserAsync(int id, UserDTO userDTO);
    Task DeleteUserAsync(int id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<string> LoginAsync(string email, string password);
    Task<User> RegisterAsync(UserDTO userDTO);
    Task LogoutAsync(string token);
    Task<string> RefreshTokenAsync(string token);
    Task<List<string>> CheckExistingEmailsAsync(List<UserDTO> userDTOList);
    Task SaveAllUsersAsync(List<UserDTO> userDTOList);
    Task<List<User>> GetUsersCreatedBetweenAsync(DateTime start, DateTime end);
    Task<Dictionary<string, long>> GetUserSegmentsAsync();
    Task<Dictionary<int, long>> CountUsersByMonthCurrentYearAsync();
    Task<List<User>> GetUsersWithExpiringSubscriptionsAsync();
    Task<User> ChangePasswordAsync(User user);
    Task DeleteSubscriptionAsync(int id);
    Task<User> FetchAsync(string token);
    Task<User> PaymentAsync(User user);
    Task<PagedResult<User>> GetUsersAsync(int page, int size, string email, string fullName, string role, string subscriptionPlan, string sort);
}
