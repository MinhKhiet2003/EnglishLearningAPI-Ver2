using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{
    public class TopicProgressRepository : ITopicProgressRepository
    {
        private readonly AppDbContext _context;

        public TopicProgressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TopicProgress> FindByUserAndTopicAsync(User user, Topic topic)
        {
            return await _context.TopicProgresses
                .FirstOrDefaultAsync(tp => tp.User.Id == user.Id && tp.Topic.Id == topic.Id);
        }

        public async Task SaveAsync(TopicProgress topicProgress)
        {
            _context.TopicProgresses.Add(topicProgress);
            await _context.SaveChangesAsync();
        }

        public async Task<List<TopicProgress>> GetAllTopicProgressForUserAsync(User user)
        {
            return await _context.TopicProgresses
                .Where(tp => tp.User.Id == user.Id)
                .ToListAsync();
        }

        public async Task<List<(string TopicName, long Count)>> FindTop10PopularTopicsAsync()
        {
            return await _context.TopicProgresses
                .GroupBy(tp => tp.Topic.TopicName)
                .Select(g => new ValueTuple<string, long>(g.Key, g.LongCount()))
                .OrderByDescending(g => g.Item2)
                .Take(10)
                .ToListAsync();
        }
    }
}
