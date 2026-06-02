using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Services
{
    public class ContractEligiblilityService : IContractEligibilityService
    {
        private readonly ApplicationDbContext _context;

        public ContractEligiblilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EligibilityResult> CheckEligibilityAsync(int contractId)
        {
            var contract = await _context.Contracts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == contractId);

            if (contract == null)
            {
                return EligibilityResult.Ineligible("Selected contract does not exist");

            }

            if (contract.Status == ContractStatus.Expired)
            {
                return EligibilityResult.Ineligible("Cannot create a service request on an expired contract");
            }

            if (contract.Status == ContractStatus.OnHold)
            {
                return EligibilityResult.Ineligible("Cannot create a service request on an on hold contract");
            }

            return EligibilityResult.Eligible();
        }
    }
}
