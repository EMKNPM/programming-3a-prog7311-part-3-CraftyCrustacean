using GLMS.Factories;
using Xunit;

namespace GLMS.Tests.Factories
{
    public class ContractFactoryResolverTests
    {
        [Theory]
        [InlineData("Freight", typeof(FreightContractFactory))]
        [InlineData("freight", typeof(FreightContractFactory))]
        [InlineData("FREIGHT", typeof(FreightContractFactory))]
        [InlineData("SLA", typeof(SLAContractFactory))]
        [InlineData("sla", typeof(SLAContractFactory))]
        [InlineData("Warehouse", typeof(WarehouseContractFactory))]
        [InlineData("warehouse", typeof(WarehouseContractFactory))]
        public void GetFactory_KnownType_ReturnsCorrectFactory(string input, Type expectedType)
        {
            var factory = ContractFactoryResolver.GetFactory(input);
            Assert.IsType(expectedType, factory);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("Unknown")]
        [InlineData("Airlift")]
        public void GetFactory_UnknownType_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => ContractFactoryResolver.GetFactory(input));
        }

        [Fact]
        public void GetFactory_NullInput_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => ContractFactoryResolver.GetFactory(null!));
        }

        [Fact]
        public void AvailableTypes_IncludesAllThreeContractTypes()
        {
            var types = ContractFactoryResolver.AvailableTypes.ToList();

            Assert.Contains("Freight", types);
            Assert.Contains("SLA", types);
            Assert.Contains("Warehouse", types);
            Assert.Equal(3, types.Count);
        }
    }
}