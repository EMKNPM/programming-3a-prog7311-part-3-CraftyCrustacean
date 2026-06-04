using GLMS.Models;

namespace GLMS.Processors
{
    public class FreightProcessor : RequestProcessor
    {
        private const decimal MaxShipmentWeightTonnes = 40m;

        protected override ProcessingResult ValidateContract(ServiceRequest request, Contract contract)
        {
            var baseResult = base.ValidateContract(request, contract);
            if (!baseResult.Success) { return baseResult; }

            if (request.WeightTonnes == null || request.WeightTonnes <= 0)
            {
                return ProcessingResult.Failed("Freight requests must declare a shipment weight in tonnes.");
            }

            if (request.WeightTonnes > MaxShipmentWeightTonnes)
            {
                return ProcessingResult.Failed($"Freight shipment weight exceeds maximum");
            }

            return ProcessingResult.Succeeded(string.Empty, "Freight contract validated successfully");
        }
    }
}
