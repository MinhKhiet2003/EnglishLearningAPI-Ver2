using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnglishLearningAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm người dùng bằng email", email);
                throw;
            }

        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra sự tồn tại của người dùng bằng email");
                throw;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy tất cả người dùng");
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy người dùng bằng ID");
                throw;
            }
        }

        public async Task<List<User>> FindUsersCreatedBetweenAsync(DateTime start, DateTime end)
        {
            try
            {
                return await _context.Users
                    .Where(u => u.CreatedAt >= start && u.CreatedAt <= end)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm người dùng được tạo giữa các ngày");
                throw;
            }
        }

        public async Task<List<(string SubscriptionPlan, long Count)>> FindSubscriptionPlanCountsAsync()
        {
            try
            {
                return await _context.Users
                    .GroupBy(u => u.SubscriptionPlan)
                    .Select(g => new ValueTuple<string, long>(g.Key, g.LongCount()))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm số lượng gói đăng ký");
                throw;
            }
        }

        public async Task<List<User>> FindUsersWithExpiringSubscriptionsAsync()
        {
            try
            {
                return await _context.Users
                    .Where(u => u.SubscriptionPlan != "none" && u.SubscriptionEndDate > DateTime.UtcNow)
                    .OrderBy(u => u.SubscriptionEndDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm người dùng có gói đăng ký sắp hết hạn");
                throw;
            }
        }

        public async Task<List<(int Month, long TotalUsers)>> CountUsersByMonthCurrentYearAsync()
        {
            try
            {
                var currentYear = DateTime.UtcNow.Year;
                return await _context.Users
                    .Where(u => u.CreatedAt.Year == currentYear)
                    .GroupBy(u => u.CreatedAt.Month)
                    .Select(g => new ValueTuple<int, long>(g.Key, g.LongCount()))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đếm người dùng theo tháng trong năm hiện tại");
                throw;
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo người dùng");
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật người dùng");
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                var user = await GetUserByIdAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa người dùng");
                throw;
            }
        }

        public async Task<List<User>> SaveAllAsync(List<User> users)
        {
            try
            {
                await _context.Users.AddRangeAsync(users);
                await _context.SaveChangesAsync();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu tất cả người dùng");
                throw;
            }
        }
    }
}
