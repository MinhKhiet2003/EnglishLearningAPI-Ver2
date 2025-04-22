using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{
    public class UserProgressRepository : IUserProgressRepository
    {
        private readonly AppDbContext _context;

        public UserProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. Lấy tất cả từ vựng cho một user
        public async Task<List<UserProgress>> FindAllVocabForUserAsync(User user)
        {
            return await _context.UserProgresses
                .Where(up => up.User.Id == user.Id)
                .ToListAsync();
        }

        // 2. Đếm số lượng từ vựng ở từng level cho một user
        public async Task<List<(int Level, long Count)>> CountLevelsByUserAsync(User user)
        {
            var result = await _context.UserProgresses
                .Where(up => up.User.Id == user.Id)
                .GroupBy(up => up.Level)
                .Select(g => new { Level = g.Key, Count = g.LongCount() })
                .ToListAsync();

            return result.Select(r => (r.Level, r.Count)).ToList();
        }

        // 3. Lấy ngẫu nhiên từ vựng để luyện tập
        public async Task<List<UserProgress>> FindAllVocabForUserWithExamAsync(int userId)
        {
            return await _context.UserProgresses
                .Where(up => up.User.Id == userId)
                .OrderBy(r => EF.Functions.Random())
                .Take(10)
                .ToListAsync();
        }

        // 4. Lấy một tiến trình cụ thể dựa trên User và Vocabulary
        public async Task<UserProgress> GetUserProgressAsync(User user, Vocabulary vocab)
        {
            return await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.User.Id == user.Id && up.Vocabulary.Id == vocab.Id);
        }

        // 5. Tìm kiếm tiến trình theo level và từ khóa
        public async Task<List<UserProgress>> FindUserProgressByLevelAsync(string search, int level, User user)
        {
            var query = _context.UserProgresses
                .Include(up => up.Vocabulary)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(up =>
                    up.Vocabulary.Word.Contains(search ?? "") ||
                    up.Vocabulary.Meaning.Contains(search ?? ""));
            }

            if (level > 0)
            {
                query = query.Where(up => up.Level == level);
            }

            return await query.Where(up => up.UserId == user.Id)
                              .OrderBy(up => up.Vocabulary.Word)
                              .ToListAsync();
        }

        // 6. Kiểm tra tồn tại tiến trình dựa trên User và Vocabulary
        public async Task<bool> ExistsByUserAndVocabularyAsync(User user, Vocabulary vocabulary)
        {
            return await _context.UserProgresses
                .AnyAsync(up => up.User.Id == user.Id && up.Vocabulary.Id == vocabulary.Id);
        }

        // 7. Thống kê 10 từ vựng phổ biến nhất
        public async Task<List<(string Word, long Count)>> FindTop10PopularVocabsAsync()
        {
            var result = await _context.UserProgresses
                .GroupBy(up => up.Vocabulary.Word)
                .Select(g => new { Word = g.Key, Count = g.LongCount() }) // Anonymous object
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            // Chuyển đổi từ anonymous object sang tuple
            return result.Select(r => (r.Word, r.Count)).ToList();
        }


        // 8. Lấy tiến trình cụ thể dựa trên User và ID
        public async Task<UserProgress> GetUserProgressByIdAsync(User user, int id)
        {
            return await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.User.Id == user.Id && up.Id == id);
        }

        // 9. Lưu nhiều tiến trình học từ vựng
        public async Task<List<UserProgress>> SaveAllAsync(List<UserProgress> userProgressList)
        {
            await _context.UserProgresses.AddRangeAsync(userProgressList);
            await _context.SaveChangesAsync();
            return userProgressList;
        }

        // 10. Cập nhật tiến trình học
        public async Task UpdateAsync(UserProgress userProgress)
        {
            _context.UserProgresses.Update(userProgress);
            await _context.SaveChangesAsync();
        }

        // 11. Xóa tiến trình học
        public async Task DeleteAsync(UserProgress userProgress)
        {
            _context.UserProgresses.Remove(userProgress);
            await _context.SaveChangesAsync();
        }

        // 12. Lấy tất cả các tiến trình học
        public async Task<List<UserProgress>> GetAllAsync()
        {
            return await _context.UserProgresses.ToListAsync();
        }
    }
}
