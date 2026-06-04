namespace GLMS.Commands
{
    public class RequestInvoker
    {
        private readonly List<ICommand> _history = new();

        public IReadOnlyList<ICommand> History => _history.AsReadOnly();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _history.Add(command);
        }

        public void UndoLast()
        {
            if (_history.Count > 0) return;

            var last = _history[^1];
            last.Undo();
            _history.RemoveAt(_history.Count - 1);
        }
    }
}
