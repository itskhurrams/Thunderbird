using Thunderbird.Domain.Entities;

namespace Thunderbird.Domain.Interfaces {
    public interface ITerritoryRepository {
        public Task<IList<Division>> GetDivisions(bool? isActive=null);
    }

}
