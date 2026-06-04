namespace GLMS.Factories
{
    //This creator interface holds the method that each contract factory will extend
    //and houses all the common fields each contract needs.
    public interface IContractFactory
    {
        IContract CreateContract(DateTime startDate, DateTime endDate, string serviceLevel);
    }
}
