using Microsoft.AspNetCore.Mvc;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Service;
using Microsoft.AspNetCore.Http;
using EnglishLearningAPI.Response;
using System.Threading.Tasks;

namespace EnglishLearningAPI.Controllers
{
    [ApiController]
    [Route("api/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IVNPAYService _vnpayService;
        private readonly IPaymentService _paymentService;

        public PaymentController(IUserService userService, IVNPAYService vnpayService, IPaymentService paymentService)
        {
            _userService = userService;
            _vnpayService = vnpayService;
            _paymentService = paymentService;
        }

        [HttpGet("pay")]
        public IActionResult Pay([FromQuery] IQueryCollection query)
        {
            var paymentUrl = _vnpayService.CreatePaymentUrl(query);
            return Ok(ApiResponse<string>.Success(200, "", paymentUrl));
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] IQueryCollection query, [FromHeader(Name = "Authorization")] string authHeader)
        {
            var status = query["vnp_ResponseCode"].ToString();
            if (status == "00")
            {
                var token = authHeader.Substring(7);
                var user = await _userService.FetchAsync(token);
                var amount = double.Parse(query["vnp_Amount"].ToString()) / 100;
                if (amount == 299000 || amount == 499000)
                {
                    user.SubscriptionPlan = "6_months";
                }
                else if (amount == 699000 || amount == 899000)
                {
                    user.SubscriptionPlan = "1_year";
                }
                else
                {
                    user.SubscriptionPlan = "3_years";
                }
                var updatedUser = await _userService.PaymentAsync(user);
                await _paymentService.CreatePaymentAsync(user, amount);
                return Ok(ApiResponse<User>.Success(200, "Thanh toán thành công", updatedUser));
            }
            else
            {
                return BadRequest(ApiResponse<string>.error(400, "Thanh toán thất bại", "Bad Request"));
            }
        }
    }
}
