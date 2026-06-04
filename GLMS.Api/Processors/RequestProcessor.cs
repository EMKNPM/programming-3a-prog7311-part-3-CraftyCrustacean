using GLMS.Models;
using System.Data;

namespace GLMS.Processors
{

    public abstract class RequestProcessor
    {
        public ProcessingResult ProcessRequest(ServiceRequest request, Contract contract)
        {
            var validation = ValidateContract(request, contract);
            if (!validation.Success)
            {
                return validation;
            }

            var availability = CheckAvailability(request, contract);
            if (!availability.Success)
            {
                return availability;
            }

            var invoice = GenerateInvoice(request, contract);
            return invoice;
        }

        protected virtual ProcessingResult ValidateContract(ServiceRequest request, Contract contract)
        {
            if (contract.Status == ContractStatus.Expired)
            {
                return ProcessingResult.Failed("Could not process: contract has expired");
            }
            if (contract.Status == ContractStatus.OnHold)
            {
                return ProcessingResult.Failed("Could not process: contract is on hold");
            }
            if (contract.Status == ContractStatus.Draft)
            {
                return ProcessingResult.Failed("Could not process: contract is still in draft");
            }
            return ProcessingResult.Succeeded(string.Empty, "Contract validated succesfuly");
        }

        protected virtual ProcessingResult CheckAvailability(ServiceRequest request, Contract contract) 
        {
            return ProcessingResult.Succeeded(string.Empty, "No availbility check needed");
        }

        protected virtual ProcessingResult GenerateInvoice(ServiceRequest request, Contract contract)
        {
            string invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{request.Id:D6}";
            return ProcessingResult.Succeeded(invoiceNumber, $"Invoice {invoiceNumber} generated for R{request.CostZAR:F2}");
        }
    }
}
