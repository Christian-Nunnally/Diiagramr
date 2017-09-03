using Diiagramr.Model;

namespace Diiagramr.Executor
{
    public class DiagramExecutor : IDiagramExecutor
    {
        private OutputTerminal StartingOutputTerminal { get; set; }

        private object StartingData { get; set; }

        public void Execute(OutputTerminal outputTerminal, object data)
        {
            StartingOutputTerminal = outputTerminal;
            StartingData = data;
            Execute();
        }

        private void Execute()
        {
            var inputTerminal = StartingOutputTerminal?.ConnectedWire?.SinkTerminal;
            var outputResult = inputTerminal?.Execute(StartingData);
            if (outputResult == null) return;
            foreach (var o in outputResult)
                Execute(o.Key, o.Value);
        }
    }
}