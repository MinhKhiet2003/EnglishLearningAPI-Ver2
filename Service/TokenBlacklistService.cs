using System.Collections.Concurrent;
namespace EnglishLearningAPI.Service.IService
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();

        public void BlacklistToken(string token)
        {
            _blacklistedTokens[token] = DateTime.UtcNow;
        }

        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.ContainsKey(token);
        }
    }
}