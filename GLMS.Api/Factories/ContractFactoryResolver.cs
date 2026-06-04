namespace GLMS.Factories
{
    //This helper decides which factory to use based on user input, seperating it here keeps the controller free of switch statements
    public static class ContractFactoryResolver
    {
        public static IContractFactory GetFactory(string contractType)
        {
            return contractType?.ToLowerInvariant() switch
            {
                "freight" => new FreightContractFactory(),
                "sla" => new SLAContractFactory(),
                "warehouse" => new WarehouseContractFactory(),
                _ => throw new ArgumentException(
                    $"Unknow contract type: '{contractType}'. " +
                    $"Valid types are: Freight, SLA, Warehouse",
                    nameof(contractType))
            };

        }
        //List of valid contract types for populating drop down lists etc.
        public static IEnumerable<string> AvailableTypes => new[] { "Freight", "SLA", "Warehouse" };
    }
}