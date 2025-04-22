using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using EnglishLearningAPI.Config;
using System.Linq;
using EnglishLearningAPI.Service.IService;

namespace EnglishLearningAPI.Services
{
    public class VNPAYService : IVNPAYService
    {
        private readonly VNPAYConfig _vnpayConfig;

        public VNPAYService(VNPAYConfig vnpayConfig)
        {
            _vnpayConfig = vnpayConfig;
        }

        public string CreatePaymentUrl(IQueryCollection query)
        {
            long amount = (long)(double.Parse(query["amount"]) * 100);
            string bankCode = query["bankCode"];
            var vnpParams = _vnpayConfig.GetVNPayConfig();
            vnpParams["vnp_Amount"] = amount.ToString();
            if (!string.IsNullOrEmpty(bankCode))
            {
                vnpParams["vnp_BankCode"] = bankCode;
            }
            vnpParams["vnp_IpAddr"] = _vnpayConfig.GetIpAddress(new DefaultHttpContext().Request);
            string queryUrl = _vnpayConfig.GetPaymentURL(vnpParams, true);
            string hashData = _vnpayConfig.GetPaymentURL(vnpParams, false);
            string vnpSecureHash = _vnpayConfig.HmacSHA512(_vnpayConfig.SecretKey, hashData);
            queryUrl += "&vnp_SecureHash=" + vnpSecureHash;
            return _vnpayConfig.Vnp_PayUrl + "?" + queryUrl;
        }
    }
}
