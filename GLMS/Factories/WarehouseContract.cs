using GLMS.Models;

namespace GLMS.Factories
{
    public class WarehouseContract : ContractBase
    {
        public override string ContractType => "Warehouse";

        public WarehouseContract(DateTime startDate, DateTime endDate, string serviceLevel)
        {
            StartDate = startDate;
            ExpiryDate = endDate;
            ServiceLevel = $"[Warehouse] {serviceLevel}";
            Status = ContractStatus.Draft;
        }
    }
}
