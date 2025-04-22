using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EnglishLearningAPI.Models;
using EnglishLearningAPI.Service.IService;
using EnglishLearningAPI.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using EnglishLearningAPI.Repositories;

namespace EnglishLearningAPI.Component
{
    public class JwtTokenUtil : IJwtTokenUtil
    {
        //private readonly TokenBlacklistService _tokenBlacklistService;
        private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly long _expiration;
        private readonly ILogger<UserRepository> _logger;

        public JwtTokenUtil(/*TokenBlacklistService tokenBlacklistService, */ ILogger<UserRepository> logger, IConfiguration configuration)
        {
            //_tokenBlacklistService = tokenBlacklistService;
            _configuration = configuration;
            _secretKey = _configuration["Jwt:SecretKey"];
            _expiration = long.Parse(_configuration["Jwt:Expiration"]);
            _logger = logger;

        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role) // Thêm claim cho vai trò người dùng
            };

            try
            {
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddSeconds(_expiration), 
                    signingCredentials: new SigningCredentials(GetSignInKey(), SecurityAlgorithms.HmacSha256)
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to generate token");
                throw new Exception($"Unable to generate token: {e.Message}");
            }
        }


        private SymmetricSecurityKey GetSignInKey()
        {
            var keyBytes = Encoding.ASCII.GetBytes(_secretKey);
            return new SymmetricSecurityKey(keyBytes);
        }


        private string GenerateSecretKey()
        {
            using (var random = new RNGCryptoServiceProvider())
            {
                var keyBytes = new byte[32];
                random.GetBytes(keyBytes);
                return Convert.ToBase64String(keyBytes);
            }
        }

        private ClaimsPrincipal ExtractAllClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSignInKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal;
        }

        public T ExtractClaim<T>(string token, Func<ClaimsPrincipal, T> claimsResolver)
        {
            var claims = ExtractAllClaims(token);
            return claimsResolver(claims);
        }

        public bool IsTokenExpired(string token)
        {
            var expirationDate = ExtractClaim(token, claims => claims.FindFirstValue(JwtRegisteredClaimNames.Exp));
            return DateTime.Parse(expirationDate).ToUniversalTime() < DateTime.UtcNow;
        }

        public string GetEmailFromToken(string token)
        {
            return ExtractClaim(token, claims => claims.FindFirstValue(ClaimTypes.Email));
        }

        public bool ValidateToken(string token, UserDetails userDetails)
        {
            //if (_tokenBlacklistService.IsTokenBlacklisted(token))
            //{
            //    return false;
            //}

            var email = GetEmailFromToken(token);
            return email == userDetails.Username && !IsTokenExpired(token);
        }
        public ClaimsPrincipal GetClaimsFromToken(string token)
        {
            return ExtractAllClaims(token);
        }
    }
}
