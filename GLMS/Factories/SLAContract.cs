using GLMS.Models;

namespace GLMS.Factories
{
    public class SLAContract : ContractBase
    {
        public override string ContractType => "SLA";

        public SLAContract(DateTime startDate, DateTime endDate, string serviceLevel)
        {
            StartDate = startDate;
            ExpiryDate = endDate;
            ServiceLevel = $"[SLA] {serviceLevel}";
            Status = ContractStatus.Draft;
        }
    }
}
