using GLMS.Models;

namespace GLMS.Commands
{
    public class RejectCommand : ICommand
    {
        private readonly ServiceRequest _request;
        public readonly string _reason;
        private ServiceRequestStatus _previousStatus;

        public string Description => $"Rejected request #{_request.Id} (reason: {_reason})";

        public RejectCommand(ServiceRequest request, string reason)
        {
            _request = request;
            _reason = reason;
        }

        public void Execute()
        {
            _previousStatus = _request.Status;
            _request.Status = ServiceRequestStatus.Rejected;
            //Idealy history and reasons would be saved to the database as well, could implement in part 3
        }

        public void Undo()
        {
            _request.Status = _previousStatus;
        }
    }
}
