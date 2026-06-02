using GLMS.Models;

namespace GLMS.Services
{
    public interface IContractEligibilityService
    {
        Task<EligibilityResult> CheckEligibilityAsync(int contractId);
    }

    public record EligibilityResult(bool isEligible, string? Reason)
    {
        public static EligibilityResult Eligible() => new(true, null);
        public static EligibilityResult Ineligible(string reason) => new(false, reason);
    }
}
