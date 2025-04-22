using System;
using System.Linq;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Repositories;

namespace EnglishLearningAPI.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task CreatePaymentAsync(User user, double price)
        {
            var payment = new Payment
            {
                Price = price,
                User = user,
                UserId = user.Id,
                CreateAt = DateTime.UtcNow
            };
            await _paymentRepository.AddAsync(payment);
        }

        public async Task<decimal> GetRevenueAsync()
        {
            var payments = await _paymentRepository.FindAllAsync();
            return payments
                .Select(payment => Math.Round((decimal)payment.Price, 2, MidpointRounding.AwayFromZero))
                .Aggregate(0m, (total, price) => total + price);
        }
    }
}
