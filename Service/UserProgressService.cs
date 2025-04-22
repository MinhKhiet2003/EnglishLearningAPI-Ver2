using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;

namespace EnglishLearningAPI.Service
{
    public class UserProgressService : IUserProgressService
    {
        private readonly IUserProgressRepository _userProgressRepository;

        public UserProgressService(IUserProgressRepository userProgressRepository)
        {
            _userProgressRepository = userProgressRepository;
        }

        public async Task<List<UserProgress>> GetAllVocabForUserAsync(User user)
        {
            return await _userProgressRepository.FindAllVocabForUserAsync(user);
        }

        public async Task<List<UserProgress>> SaveAllVocabForUserAsync(User user, List<Vocabulary> vocabs)
        {
            var userProgressList = vocabs.Select(vocab => new UserProgress
            {
                User = user,
                Vocabulary = vocab,
                Level = 1,
                LastReviewed = DateTime.UtcNow,
                NextReview = DateTime.UtcNow.AddHours(1)
            }).ToList();

            return await _userProgressRepository.SaveAllAsync(userProgressList);
        }

        public async Task<Dictionary<int, long>> CountLevelsByUserAsync(User user)
        {
            var results = await _userProgressRepository.CountLevelsByUserAsync(user);
            return results.ToDictionary(r => r.Level, r => r.Count);
        }

        public async Task<List<UserProgress>> GetAllVocabForUserWithExamAsync(User user)
        {
            return await _userProgressRepository.FindAllVocabForUserWithExamAsync(user.Id);
        }

        public async Task<UserProgress> GetUserProgressAsync(User user, Vocabulary vocabulary)
        {
            return await _userProgressRepository.GetUserProgressAsync(user, vocabulary)
                   ?? throw new Exception("Không tìm thấy tiến độ học từ vựng.");
        }

        public async Task<UserProgress> UpdateUserProgressAsync(UserProgress userProgress, int status)
        {
            userProgress.LastReviewed = DateTime.UtcNow;

            if (status == 1) // Trả lời đúng
            {
                if (userProgress.Level == 5)
                {
                    userProgress.NextReview = DateTime.UtcNow.AddMonths(1);
                }
                else
                {
                    userProgress.Level++;
                    userProgress.NextReview = UpdateNextReviewDate(userProgress.Level);
                }
            }
            else // Trả lời sai
            {
                userProgress.Level = 1;
                userProgress.NextReview = DateTime.UtcNow.AddHours(1);
            }

            await _userProgressRepository.UpdateAsync(userProgress);
            return userProgress;
        }

        public async Task<bool> IsVocabExistForUserAsync(User user, Vocabulary vocab)
        {
            return await _userProgressRepository.ExistsByUserAndVocabularyAsync(user, vocab);
        }

        public async Task<bool> AllVocabulariesAssignedToUserAsync(User user, List<Vocabulary> vocabularies)
        {
            foreach (var vocab in vocabularies)
            {
                if (!await _userProgressRepository.ExistsByUserAndVocabularyAsync(user, vocab))
                    return false;
            }
            return true;
        }

        public async Task DeleteUserProgressAsync(User user, int id)
        {
            var userProgress = await _userProgressRepository.GetUserProgressByIdAsync(user, id)
                                ?? throw new Exception($"Không tìm thấy với id: {id}");
            await _userProgressRepository.DeleteAsync(userProgress);
        }

        public async Task<Dictionary<string, long>> GetTop10PopularVocabsAsync()
        {
            var results = await _userProgressRepository.FindTop10PopularVocabsAsync();
            return results.Take(10).ToDictionary(r => r.Word, r => r.Count);
        }

        public async Task<List<UserProgress>> GetUserProgressByLevelAsync(string search, int level, User user)
        {
            return await _userProgressRepository.FindUserProgressByLevelAsync(search, level, user);
        }

        public async Task ScheduledUserProgressCleanupAsync()
        {
            var allProgresses = await _userProgressRepository.GetAllAsync();

            foreach (var progress in allProgresses)
            {
                var now = DateTime.UtcNow;

                var reviewTime = progress.LastReviewed.GetValueOrDefault().Add(
                progress.Level switch
                {
                    1 => TimeSpan.FromDays(1),
                    2 => TimeSpan.FromDays(3),
                    3 => TimeSpan.FromDays(7),
                    4 => TimeSpan.FromDays(14),
                    _ => TimeSpan.FromDays(30),
                });



                if (reviewTime < now)
                {
                    if (progress.Level == 1)
                    {
                        await _userProgressRepository.DeleteAsync(progress);
                    }
                    else
                    {
                        await UpdateUserProgressAsync(progress, 0); // Trả lời sai, hạ cấp
                    }
                }
            }
        }

        private DateTime UpdateNextReviewDate(int level)
        {
            return level switch
            {
                1 => DateTime.UtcNow.AddHours(1),
                2 => DateTime.UtcNow.AddDays(1),
                3 => DateTime.UtcNow.AddDays(3),
                4 => DateTime.UtcNow.AddDays(7),
                5 => DateTime.UtcNow.AddMonths(1),
                _ => DateTime.UtcNow.AddHours(1)
            };
        }
    }
}
