using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{
    public class VocabularyRepository : IVocabularyRepository
    {
        private readonly AppDbContext _context;

        public VocabularyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vocabulary> AddAsync(Vocabulary vocabulary)
        {
            await _context.Vocabularies.AddAsync(vocabulary);
            await _context.SaveChangesAsync();
            return vocabulary;
        }

        public async Task<Vocabulary> GetByIdAsync(int id)
        {
            return await _context.Vocabularies.AsNoTracking().FirstAsync(x => x.Id == id);
        }

        public async Task<List<Vocabulary>> GetAllAsync()
        {
            return await _context.Vocabularies.ToListAsync();
        }

        public async Task<Vocabulary> UpdateAsync(Vocabulary vocabulary)
        {
            _context.Vocabularies.Update(vocabulary);
            await _context.SaveChangesAsync();
            return vocabulary;
        }

        public async Task DeleteAsync(Vocabulary vocabulary)
        {
            _context.Vocabularies.Remove(vocabulary);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByWordAsync(string word)
        {
            return await _context.Vocabularies.AnyAsync(v => v.Word == word);
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _context.Vocabularies.AnyAsync(v => v.Id == id);
        }

        public async Task<List<int>> GetExistingIdsAsync(List<int> ids)
        {
            return await _context.Vocabularies.Where(v => ids.Contains(v.Id)).AsNoTracking().Select(v => v.Id).ToListAsync();
        }

        public async Task<List<string>> GetExistingWordsAsync(List<string> words)
        {
            return await _context.Vocabularies.Where(v => words.Contains(v.Word)).Select(v => v.Word).ToListAsync();
        }

        public async Task AddRangeAsync(List<Vocabulary> vocabularies)
        {
            await _context.Vocabularies.AddRangeAsync(vocabularies);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vocabulary>> FindTwoRandomVocabsAsync(int vocabId, int topicId)
        {
            return await _context.Vocabularies
                .Where(v => v.Id != vocabId && v.TopicId == topicId)
                .OrderBy(r => EF.Functions.Random())
                .Take(2)
                .ToListAsync();
        }

        public IQueryable<Vocabulary> Query()
        {
            return _context.Vocabularies.Include(x => x.Topic).AsNoTracking().AsQueryable();
        }
    }
}
