namespace GLMS.Factories
{
    public class SLAContractFactory : IContractFactory
    {
        public IContract CreateContract(DateTime startDate, DateTime endDate, string serviceLevel)
        {
            return new SLAContract(startDate, endDate, serviceLevel);
        }
    }
}