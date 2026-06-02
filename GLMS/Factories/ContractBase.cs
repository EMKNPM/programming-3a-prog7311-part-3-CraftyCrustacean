using GLMS.Models;

namespace GLMS.Factories
{

    public abstract class ContractBase : IContract
    {
        public int ContractId { get; protected set; }
        public ContractStatus Status { get; protected set; }
        public DateTime ExpiryDate { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public string ServiceLevel { get; protected set; } = string.Empty;
        public abstract string ContractType { get; }

        public Contract ToEntity(int clientId)
        {
            return new Contract
            {
                ClientId = clientId,
                StartDate = this.StartDate,
                EndDate = this.ExpiryDate,
                Status = this.Status,
                ServiceLevel = this.ServiceLevel

            };
        }
    }

}
