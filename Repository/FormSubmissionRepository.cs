using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Data;
using EnglishLearningAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Repositories
{
    public class FormSubmissionRepository : IFormSubmissionRepository
    {
        private readonly AppDbContext _context;

        public FormSubmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        // Lấy FormSubmission theo ID
        public async Task<FormSubmission> GetByIdAsync(int id)
        {
            return await _context.FormSubmissions.FindAsync(id);
        }

        // Lấy tất cả FormSubmissions
        public async Task<List<FormSubmission>> GetAllAsync()
        {
            return await _context.FormSubmissions.ToListAsync();
        }

        // Thêm mới FormSubmission
        public async Task AddAsync(FormSubmission formSubmission)
        {
            await _context.FormSubmissions.AddAsync(formSubmission);
        }

        // Cập nhật FormSubmission
        public void Update(FormSubmission formSubmission)
        {
            _context.FormSubmissions.Update(formSubmission);
        }

        // Xóa FormSubmission
        public void Delete(FormSubmission formSubmission)
        {
            _context.FormSubmissions.Remove(formSubmission);
        }

        // Lưu thay đổi vào cơ sở dữ liệu
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Truy vấn FormSubmissions
        public IQueryable<FormSubmission> Query()
        {
            return _context.FormSubmissions.Include(x => x.User).AsQueryable();
        }
    }
}
