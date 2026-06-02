namespace GLMS.Factories
{
    public class FreightContractFactory : IContractFactory
    {
        public IContract CreateContract(DateTime startDate, DateTime endDate, string serviceLevel)
        {
            return new FreightContract(startDate, endDate, serviceLevel);
        }
    }
}
