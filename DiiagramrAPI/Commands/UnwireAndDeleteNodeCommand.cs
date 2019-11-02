using DiiagramrAPI.Editor;
using DiiagramrAPI.Application.Commands;

namespace DiiagramrAPI.Commands
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