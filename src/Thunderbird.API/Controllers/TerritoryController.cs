using Thunderbird.Application.Interfaces;
using Thunderbird.Domain.Entities;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thunderbird.API.Controllers {
    public class TerritoryController : BaseController {
        private readonly ITerritoryService _territoryService;
        public TerritoryController(ITerritoryService territoryService) {
            _territoryService = territoryService;
        }
        [HttpGet]
        public async Task<IList<Division>> GetDivisions(bool? isActive) {
            return await _territoryService.GetDivisions(isActive);
        }
    }
}
