using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> FindUsersCreatedBetweenAsync(DateTime start, DateTime end);
        Task<List<(string SubscriptionPlan, long Count)>> FindSubscriptionPlanCountsAsync();
        Task<List<User>> FindUsersWithExpiringSubscriptionsAsync();
        Task<List<(int Month, long TotalUsers)>> CountUsersByMonthCurrentYearAsync();
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<List<User>> SaveAllAsync(List<User> users);
    }
}
