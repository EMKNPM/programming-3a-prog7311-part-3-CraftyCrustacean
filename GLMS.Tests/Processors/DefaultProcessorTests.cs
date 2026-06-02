using GLMS.Models;
using GLMS.Processors;
using Xunit;

namespace GLMS.Tests.Processors
{
    public class DefaultProcessorTests
    {
        [Theory]
        [InlineData(ContractStatus.Active, true)]
        [InlineData(ContractStatus.OnHold, false)]
        [InlineData(ContractStatus.Expired, false)]
        [InlineData(ContractStatus.Draft, false)]
        public void ProcessRequest_VariousStatuses_BehavesExpected(ContractStatus status, bool expectedSuccess)
        {
            var processor = new DefaultProcessor();
            var contract = new Contract
            {
                Id = 1,
                Status = status,
                ServiceLevel = "[SLA] Premium",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(6)
            };
            var request = new ServiceRequest
            {
                Id = 1,
                CostZAR = 5000m
            };

            var result = processor.ProcessRequest(request, contract);
            Assert.Equal(expectedSuccess, result.Success);
        }

        [Fact]
        public void ProcessRequest_ActiveContract_GeneratesInvoiceNumber()
        {
            var processor = new DefaultProcessor();
            var contract = new Contract
            {
                Id = 1,
                Status = ContractStatus.Active,
                ServiceLevel = "[SLA] Standard"
            };
            var request = new ServiceRequest
            {
                Id = 123,
                CostZAR = 1000m
            };

            var result = processor.ProcessRequest(request, contract);

            Assert.True(result.Success);
            Assert.NotNull(result.InvoiceNumber);
            Assert.Contains(request.Id.ToString("D6"), result.InvoiceNumber);
        }
    }
}