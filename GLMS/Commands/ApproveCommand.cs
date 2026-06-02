using GLMS.Models;
using GLMS.Processors;

namespace GLMS.Commands
{
    public class ApproveCommand : ICommand
    {
        private readonly ServiceRequest _request;
        private readonly Contract _contract;
        private ServiceRequestStatus _previousStatus;
        private string? _invoiceNumber;

        public string Description => _invoiceNumber == null ? $"Approved Request #{_request.Id}" : $"Approved Request #{_request.Id} (invoice: {_invoiceNumber})";

        public ProcessingResult? LastResult {  get; private set; } 

        public ApproveCommand(ServiceRequest request, Contract contract)
        {
            _request = request;
            _contract = contract;
        }

        public void Execute()
        {
            _previousStatus = _request.Status;

            var processor = ProcessorResolver.GetProcessor(_contract.ServiceLevel);
            LastResult = processor.ProcessRequest(_request, _contract);

            if (LastResult.Success)
            {
                _request.Status = ServiceRequestStatus.Approved;
                _invoiceNumber = LastResult.InvoiceNumber;
            }
        }

        public void Undo()
        {
            _request.Status = _previousStatus;
        }
    }
}
