using GLMS.Models;

namespace GLMS.Processors
{
    public class WarehouseProcessor : RequestProcessor
    {
        private const int MaxActiveRequestPerContract = 10; //No database of warehouses and their live capacity so will just use a random number

        protected override ProcessingResult CheckAvailability(ServiceRequest request, Contract contract)
        {
            int activeRequests = contract.ServiceRequests?.Count(r => r.Status == ServiceRequestStatus.Approved) ?? 0;

            if (activeRequests >= MaxActiveRequestPerContract)
            {
                return ProcessingResult.Failed($"Warehouse at capacity contract already has {activeRequests} active requests, the maximum is {MaxActiveRequestPerContract}");
            }
            return ProcessingResult.Succeeded(string.Empty, $"Warehouse capactiy confirmed, you are using {activeRequests} out of {MaxActiveRequestPerContract} slots");
        }

    }
}
