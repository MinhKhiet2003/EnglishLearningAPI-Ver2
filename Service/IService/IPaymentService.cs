using System.Threading.Tasks;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Service
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(User user, double price);
        Task<decimal> GetRevenueAsync();
    }
}
