using Microsoft.EntityFrameworkCore;

namespace EnglishLearningAPI.Models
{
    public class PagedResult<T>
    {
        // Dữ liệu của trang hiện tại
        public List<T> Content { get; set; } = new List<T>();
        public Page Page { get; set; } = new Page();

        public PagedResult(List<T> items, int count, int currentPage, int pageSize)
        {
            Content = items;
            Page.Size = pageSize;
            Page.Number = currentPage;
            Page.TotalPages = (int)Math.Ceiling((double)items.Count() / pageSize);
            Page.TotalElements = items.Count();
        }

        // Tạo PagedResult từ danh sách và thông tin phân trang
        public static PagedResult<T> Create(List<T> source, int currentPage, int pageSize)
        {
            var count = source.Count;
            var items = source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>(items, count, currentPage, pageSize);
        }
        public static async Task<PagedResult<T>> CreateAsync(IQueryable<T> source, int page, int pageSize)
        {
            var totalCount = await source.CountAsync(); // Lấy tổng số bản ghi
            var items = await source.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync(); // Lấy bản ghi theo trang

            return new PagedResult<T>(items, totalCount, page, pageSize);
        }

        public static async Task<PagedResult<T>> CreateAsync(List<T> source, int page, int pageSize)
        {
            var totalCount = source.Count(); // Lấy tổng số bản ghi
            var items = source.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList(); // Lấy bản ghi theo trang

            return new PagedResult<T>(items, totalCount, page, pageSize);
        }
    }

    public class Page
    {
        // Trang hiện tại
        public int Size { get; set; }

        // Tổng số phần tử trong toàn bộ kết quả
        public int Number { get; set; }

        // Tổng số trang
        public int TotalPages { get; set; }

        // Kích thước trang (số phần tử trên mỗi trang)
        public int TotalElements { get; set; }
    }

}
