using GLMS.Models;
using GLMS.Processors;
using Xunit;

namespace GLMS.Tests.Processors
{
    public class WarehouseProcessorTests
    {
        private Contract MakeActiveContract(int approvedRequestCount = 0)
        {
            var contract = new Contract
            {
                Id = 1,
                Status = ContractStatus.Active,
                ServiceLevel = "[Warehouse] Cold Storage",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(12),
                ServiceRequests = new List<ServiceRequest>()
            };

            for (int i = 0; i < approvedRequestCount; i++)
            {
                contract.ServiceRequests.Add(new ServiceRequest
                {
                    Status = ServiceRequestStatus.Approved
                });
            }

            return contract;
        }

        [Fact]
        public void ProcessRequest_UnderCapacity_Succeeds()
        {
            var processor = new WarehouseProcessor();
            var contract = MakeActiveContract(approvedRequestCount: 3);
            var request = new ServiceRequest { Id = 1 };

            var result = processor.ProcessRequest(request, contract);
            Assert.True(result.Success);
            Assert.NotNull(result.InvoiceNumber);
        }

        [Fact]
        public void ProcessRequest_AtCapacity_Fails()
        {
            var processor = new WarehouseProcessor();
            var contract = MakeActiveContract(approvedRequestCount: 10);
            var request = new ServiceRequest { Id = 1 };
            var result = processor.ProcessRequest(request, contract);

            Assert.False(result.Success);
            Assert.Contains("at capacity", result.Message);
        }

        [Fact]
        public void ProcessRequest_DraftContract_FailsBeforeCapacityCheck()
        {

            var processor = new WarehouseProcessor();
            var contract = MakeActiveContract(approvedRequestCount: 10);
            contract.Status = ContractStatus.Draft;
            var request = new ServiceRequest { Id = 1 };

            var result = processor.ProcessRequest(request, contract);

            Assert.False(result.Success);
            Assert.Contains("draft", result.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}