using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Tests.TestDoubles {
    public class FakeMemoryCacheProvider : IMemoryCacheProvider {
        private readonly Dictionary<string, object> _store = new();

        public T? GetFromCache<T>(string key) where T : class =>
            _store.TryGetValue(key, out var value) ? value as T : null;

        public void SetCache<T>(string key, T value) where T : class => _store[key] = value;

        public void SetCache<T>(string key, T value, DateTimeOffset duration) where T : class => _store[key] = value;

        public void ClearCache(string key) => _store.Remove(key);
    }
}
