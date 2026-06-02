using GLMS.Models;

namespace GLMS.Factories
{
    public class FreightContract : ContractBase
    {
        public override string ContractType => "Freight";

        public FreightContract(DateTime startDate, DateTime endDate, string serviceLevel)
        {
            StartDate = startDate;
            ExpiryDate = endDate;
            ServiceLevel = $"[Freight] {serviceLevel}";
            Status = ContractStatus.Draft;
        }
    }
}
