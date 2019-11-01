using DiiagramrAPI.Shell.EditorCommands;

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
