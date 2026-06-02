using GLMS.Models;
using GLMS.Processors;
using Xunit;

namespace GLMS.Tests.Processors
{
    public class FreightProcessorTests
    {
        private Contract MakeActiveContract() => new Contract
        {
            Id = 1,
            Status = ContractStatus.Active,
            ServiceLevel = "[Freight] Standard",
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddMonths(12)
        };

        [Fact]
        public void ProcessRequest_ValidWeight_Succeeds()
        {
            var processor = new FreightProcessor();
            var contract = MakeActiveContract();
            var request = new ServiceRequest
            {
                Id = 100,
                WeightTonnes = 20m,
                CostUSD = 500m,
                CostZAR = 9250m
            };
            var result = processor.ProcessRequest(request, contract);

            Assert.True(result.Success);
            Assert.NotNull(result.InvoiceNumber);
            Assert.StartsWith("INV-", result.InvoiceNumber);
        }

        [Fact]
        public void ProcessRequest_WeightOverLimit_FailsWithMessage()
        {
            var processor = new FreightProcessor();
            var contract = MakeActiveContract();
            var request = new ServiceRequest { WeightTonnes = 50m };
            var result = processor.ProcessRequest(request, contract);

            Assert.False(result.Success);
            Assert.Contains("exceeds maximum", result.Message);
        }

        [Fact]
        public void ProcessRequest_NullWeight_FailsWithMessage()
        {

            var processor = new FreightProcessor();
            var contract = MakeActiveContract();
            var request = new ServiceRequest { WeightTonnes = null };

            var result = processor.ProcessRequest(request, contract);

            Assert.False(result.Success);
            Assert.Contains("must declare", result.Message);
        }

        [Fact]
        public void ProcessRequest_ZeroWeight_FailsWithMessage()
        {
            var processor = new FreightProcessor();
            var contract = MakeActiveContract();
            var request = new ServiceRequest { WeightTonnes = 0m };
            var result = processor.ProcessRequest(request, contract);

            Assert.False(result.Success);
            Assert.Contains("must declare", result.Message);
        }

        [Fact]
        public void ProcessRequest_ExpiredContract_FailsBeforeWeightCheck()
        {
            var processor = new FreightProcessor();
            var contract = MakeActiveContract();
            contract.Status = ContractStatus.Expired;
            var request = new ServiceRequest { WeightTonnes = 999m };

            var result = processor.ProcessRequest(request, contract);
            Assert.False(result.Success);
            Assert.Contains("expired", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("exceeds maximum", result.Message);
        }
    }
}