using Thunderbird.Domain.Interfaces;

using Microsoft.Extensions.Caching.Memory;

namespace Thunderbird.Infrastructure.Caching {
    public class MemoryCacheProvider : IMemoryCacheProvider {
        private const int CacheSeconds = 10; // 10 Seconds
        private readonly IMemoryCache _memorycache;
        public MemoryCacheProvider(IMemoryCache memorycache) {
            _memorycache = memorycache;
        }

        public T GetFromCache<T>(string key) where T : class {
            var cachedResponse = _memorycache.Get(key);
            return cachedResponse as T;
        }

        public void SetCache<T>(string key, T value) where T : class {
            SetCache(key, value, DateTimeOffset.Now.AddSeconds(CacheSeconds));
        }

        public void SetCache<T>(string key, T value, DateTimeOffset duration) where T : class {
            _memorycache.Set(key, value, duration);
        }

        public void ClearCache(string key) {
            _memorycache.Remove(key);
        }
    }
}
