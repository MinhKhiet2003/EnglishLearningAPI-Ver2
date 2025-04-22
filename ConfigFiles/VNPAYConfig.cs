using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace EnglishLearningAPI.Config
{
    public class VNPAYConfig
    {
        private readonly IConfiguration _configuration;

        public VNPAYConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Vnp_PayUrl => _configuration["vnp:payUrl"];
        public string Vnp_TmnCode => _configuration["vnp:tmnCode"];
        public string SecretKey => _configuration["vnp:secretKey"];
        public string Vnp_Version => _configuration["vnp:version"];
        public string Vnp_Command => _configuration["vnp:command"];
        public string ReturnUrl => _configuration["vnp:returnUrl"];
        public string OrderType => _configuration["vnp:orderType"];

        public Dictionary<string, string> GetVNPayConfig()
        {
            var vnpParamsMap = new Dictionary<string, string>
            {
                { "vnp_Version", Vnp_Version },
                { "vnp_Command", Vnp_Command },
                { "vnp_TmnCode", Vnp_TmnCode },
                { "vnp_CurrCode", "VND" },
                { "vnp_TxnRef", GetRandomNumber(8) },
                { "vnp_OrderInfo", "Thanh toan don hang:" + GetRandomNumber(8) },
                { "vnp_OrderType", OrderType },
                { "vnp_Locale", "vn" },
                { "vnp_ReturnUrl", ReturnUrl }
            };

            var calendar = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time");
            vnpParamsMap["vnp_CreateDate"] = calendar.ToString("yyyyMMddHHmmss");
            vnpParamsMap["vnp_ExpireDate"] = calendar.AddMinutes(15).ToString("yyyyMMddHHmmss");

            return vnpParamsMap;
        }

        public string HmacSHA512(string key, string data)
        {
            if (key == null || data == null)
            {
                throw new ArgumentNullException();
            }

            using (var hmac512 = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);
                var result = hmac512.ComputeHash(dataBytes);
                return BitConverter.ToString(result).Replace("-", "").ToLower();
            }
        }

        public string GetIpAddress(HttpRequest request)
        {
            var ipAddress = request.Headers["X-FORWARDED-FOR"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress;
        }

        public string GetRandomNumber(int len)
        {
            var rnd = new Random();
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, len).Select(s => s[rnd.Next(s.Length)]).ToArray());
        }

        public string GetPaymentURL(Dictionary<string, string> paramsMap, bool encodeKey)
        {
            return string.Join("&", paramsMap
                .Where(entry => !string.IsNullOrEmpty(entry.Value))
                .OrderBy(entry => entry.Key)
                .Select(entry => $"{(encodeKey ? WebUtility.UrlEncode(entry.Key) : entry.Key)}={WebUtility.UrlEncode(entry.Value)}"));
        }
    }
}
