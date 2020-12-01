using DiiagramrAPI.Application.Commands;
using DiiagramrAPI.Editor.Diagrams;

namespace DiiagramrAPI.Commands
{
    /// <summary>
    /// An undoable command that removes all of the wires connected to a node and then deletes the node.
    /// </summary>
    public class UnwireAndDeleteNodeCommand : BasicComposingCommand
    {
        /// <summary>
        /// Creates a new instance of <see cref="UnwireAndDeleteNodeCommand"/>.
        /// </summary>
        /// <param name="diagram">The diagram to unwire and delete from.</param>
        public UnwireAndDeleteNodeCommand(Diagram diagram)
        {
            ComposeCommand(new UnwireNodeCommand(diagram));
            ComposeCommand(new DeleteNodeCommand(diagram));
        }
    }
}