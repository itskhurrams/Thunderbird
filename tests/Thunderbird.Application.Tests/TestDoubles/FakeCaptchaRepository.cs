using Thunderbird.Domain.Interfaces;

namespace Thunderbird.Application.Tests.TestDoubles {
    public class FakeCaptchaRepository : ICaptchaRepository {
        private readonly Dictionary<long, string> _codesById = new();
        private long _nextId = 1;

        public int InsertCallCount { get; private set; }
        public int IsValidCallCount { get; private set; }

        public Task<long> Insert(string captchCode) {
            InsertCallCount++;
            long id = _nextId++;
            _codesById[id] = captchCode;
            return Task.FromResult(id);
        }

        public Task<bool> IsValid(long id, string captchCode) {
            IsValidCallCount++;
            bool isValid = _codesById.TryGetValue(id, out var storedCode) && storedCode == captchCode;
            return Task.FromResult(isValid);
        }
    }
}
