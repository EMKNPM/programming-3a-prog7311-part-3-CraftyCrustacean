using GLMS.Models;

namespace GLMS.Commands
{
    public class CancelCommand : ICommand
    {
        private readonly ServiceRequest _request;
        private ServiceRequestStatus _previousStatus;

        public string Description => $"Cancelled Request #{_request.Id}";

        public CancelCommand(ServiceRequest request)
        {
            _request = request;
        }

        public void Execute()
        {
            _previousStatus = _request.Status;
            _request.Status = ServiceRequestStatus.Cancelled;
        }

        public void Undo()
        {
            _request.Status = _previousStatus;
        }
    }
}
