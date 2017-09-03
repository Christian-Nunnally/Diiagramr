using Diiagramr.Model;

namespace Diiagramr.Executor
{
    public interface IDiagramExecutor
    {
        void Execute(OutputTerminal outputTerminal, object data);
    }
}