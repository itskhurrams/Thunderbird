using Thunderbird.Domain.Entities;

namespace Thunderbird.Application.Interfaces {
    public interface ITerritoryService {
        public Task<IList<Division>> GetDivisions(bool? isActive = null);
    }
}
