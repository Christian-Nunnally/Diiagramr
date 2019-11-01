using DiiagramrAPI.Shell.Commands;

namespace DiiagramrAPI.Diagram.Commands
{
    public class UnwireAndDeleteNodeCommand : BasicComposingCommand
    {
        public UnwireAndDeleteNodeCommand(Diagram diagram)
        {
            ComposeCommand(new UnwireNodeCommand(diagram));
            ComposeCommand(new DeleteNodeCommand(diagram));
        }
    }
}
