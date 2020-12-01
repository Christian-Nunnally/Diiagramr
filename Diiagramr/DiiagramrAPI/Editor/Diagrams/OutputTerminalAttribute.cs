using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// An attribute applied to properties in implementations of <see cref="Node"/> to turn those properties into output terminals.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputTerminalAttribute : TerminalAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="OutputTerminalAttribute"/>.
        /// </summary>
        /// <param name="defaultDirection">The default direction this terminal should face.</param>
        public OutputTerminalAttribute(Direction defaultDirection) : base(defaultDirection)
        {
        }
    }
}