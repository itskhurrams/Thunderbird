using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;
using Thunderbird.Domain.Interfaces;
using Thunderbird.Domain.Models;

namespace Thunderbird.Application.Services {
    public class TerritoryService : ITerritoryService {
        private readonly ITerritoryRepository _territoryRepository;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        private static readonly SemaphoreSlim GetUsersSemaphore = new SemaphoreSlim(1, 1);
        public TerritoryService(ITerritoryRepository territoryRepository, IMemoryCacheProvider memoryCacheProvider) {
            _territoryRepository = territoryRepository;
            _memoryCacheProvider = memoryCacheProvider;
        }
        public async Task<IList<Division>> GetDivisions(bool? isActive = null) {
            try {
                 return await GetDivisionCachedResponse(MemoryCacheKeys.Division, GetUsersSemaphore, () => _territoryRepository.GetDivisions(isActive));
            }
            catch {
                throw;
            }
        }
        private async Task<IList<Division>> GetDivisionCachedResponse(string cacheKey, SemaphoreSlim semaphore, Func<Task<IList<Division>>> func) {

            var divisionList = _memoryCacheProvider.GetFromCache<IList<Division>>(cacheKey);

            if (divisionList != null) return divisionList;
            try {
                await semaphore.WaitAsync();
                divisionList = _memoryCacheProvider.GetFromCache<IList<Division>>(cacheKey); // Recheck to make sure it didn't populate before entering semaphore
                if (divisionList != null) {
                    return divisionList;
                }
                divisionList = await func();
                _memoryCacheProvider.SetCache(cacheKey, divisionList, DateTimeOffset.Now.AddDays(1));
            }
            finally {
                semaphore.Release();
            }
            return divisionList;
        }
    }
}
