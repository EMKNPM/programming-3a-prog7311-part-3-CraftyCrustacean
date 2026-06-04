namespace GLMS.Processors
{
    public class ProcessorResolver
    {
        public static RequestProcessor GetProcessor(string serviceLevel)
        {
            if (string.IsNullOrWhiteSpace(serviceLevel))
            {
                return new DefaultProcessor();
            }

            if (serviceLevel.StartsWith("[Freight]", StringComparison.OrdinalIgnoreCase))
            {
                return new FreightProcessor();
            }

            if (serviceLevel.StartsWith("[Warehouse]", StringComparison.OrdinalIgnoreCase))
            {
                return new WarehouseProcessor();
            }

            return new DefaultProcessor();
        }
    }
}
