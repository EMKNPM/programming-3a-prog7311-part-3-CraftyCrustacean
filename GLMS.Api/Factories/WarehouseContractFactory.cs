namespace GLMS.Factories
{
    public class WarehouseContractFactory : IContractFactory
    {
        public IContract CreateContract(DateTime startDate, DateTime endDate, string serviceLevel)
        {
            return new WarehouseContract(startDate, endDate, serviceLevel);
        }
    }
}