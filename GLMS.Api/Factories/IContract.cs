using GLMS.Models;

namespace GLMS.Factories
{
    public interface IContract
    {
        int ContractId { get; }
        ContractStatus Status { get; }
        DateTime ExpiryDate { get; }
        Contract ToEntity(int clientId);
    }
}
