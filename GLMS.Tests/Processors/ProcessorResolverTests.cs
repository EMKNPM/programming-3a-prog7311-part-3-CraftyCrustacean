using GLMS.Processors;
using Xunit;

namespace GLMS.Tests.Processors
{
    public class ProcessorResolverTests
    {
        [Theory]
        [InlineData("[Freight] Standard", typeof(FreightProcessor))]
        [InlineData("[Warehouse] Cold Storage", typeof(WarehouseProcessor))]
        [InlineData("[SLA] Premium", typeof(DefaultProcessor))]
        [InlineData("", typeof(DefaultProcessor))]
        public void GetProcessor_ServiceLevelVariants_ReturnsExpectedType(string serviceLevel, Type expected)
        {
            var processor = ProcessorResolver.GetProcessor(serviceLevel);

            Assert.IsType(expected, processor);
        }
    }
}