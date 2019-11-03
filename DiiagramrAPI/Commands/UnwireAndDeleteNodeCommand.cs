using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;

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