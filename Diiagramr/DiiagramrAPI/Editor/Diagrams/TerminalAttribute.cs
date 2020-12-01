using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// Base class for defining terminals from <see cref="Node"/> properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class TerminalAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="TerminalAttribute"/>.
        /// </summary>
        /// <param name="defaultDirection">The default direction this terminal should face.</param>
        public TerminalAttribute(Direction defaultDirection)
        {
            DefaultDirection = defaultDirection;
        }

        /// <summary>
        /// The default direction this terminal should face.
        /// </summary>
        public Direction DefaultDirection { get; }
    }
}