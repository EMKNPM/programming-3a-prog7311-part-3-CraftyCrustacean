using GLMS.Factories;
using GLMS.Models;
using Xunit;

namespace GLMS.Tests.Factories
{
    public class ContractFactoryTests
    {
        private static readonly DateTime TestStart = new DateTime(2026, 1, 1);
        private static readonly DateTime TestEnd = new DateTime(2026, 12, 31);

        [Fact]
        public void FreightContractFactory_CreatesFreightContract_WithPrefixedServiceLevel()
        {
            IContractFactory factory = new FreightContractFactory();

            IContract result = factory.CreateContract(TestStart, TestEnd, "Standard");

            Assert.IsType<FreightContract>(result);
            Assert.Contains("[Freight]", ((FreightContract)result).ServiceLevel);
            Assert.Contains("Standard", ((FreightContract)result).ServiceLevel);
            Assert.Equal(ContractStatus.Draft, result.Status);
        }

        [Fact]
        public void SLAContractFactory_CreatesSLAContract_WithPrefixedServiceLevel()
        {
            IContractFactory factory = new SLAContractFactory();
            IContract result = factory.CreateContract(TestStart, TestEnd, "Premium");

            Assert.IsType<SLAContract>(result);
            Assert.Contains("[SLA]", ((SLAContract)result).ServiceLevel);
            Assert.Contains("Premium", ((SLAContract)result).ServiceLevel);
            Assert.Equal(ContractStatus.Draft, result.Status);
        }

        [Fact]
        public void WarehouseContractFactory_CreatesWarehouseContract_WithPrefixedServiceLevel()
        {
            IContractFactory factory = new WarehouseContractFactory();
            IContract result = factory.CreateContract(TestStart, TestEnd, "Cold Storage");

            Assert.IsType<WarehouseContract>(result);
            Assert.Contains("[Warehouse]", ((WarehouseContract)result).ServiceLevel);
            Assert.Contains("Cold Storage", ((WarehouseContract)result).ServiceLevel);
            Assert.Equal(ContractStatus.Draft, result.Status);
        }

        [Fact]
        public void FreightContract_ToEntity_ProducesContractWithGivenClientId()
        {
            IContractFactory factory = new FreightContractFactory();
            IContract abstraction = factory.CreateContract(TestStart, TestEnd, "Express");

            Contract entity = abstraction.ToEntity(clientId: 42);

            Assert.Equal(42, entity.ClientId);
            Assert.Equal(TestStart, entity.StartDate);
            Assert.Equal(TestEnd, entity.EndDate);
            Assert.Contains("[Freight]", entity.ServiceLevel);
            Assert.Equal(ContractStatus.Draft, entity.Status);
        }
    }
}