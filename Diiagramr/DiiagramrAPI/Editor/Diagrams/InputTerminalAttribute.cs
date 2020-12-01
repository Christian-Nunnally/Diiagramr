using DiiagramrModel;
using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// An attribute applied to properties in implementations of <see cref="Node"/> to turn those properties into input terminals.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InputTerminalAttribute : TerminalAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="InputTerminalAttribute"/>.
        /// </summary>
        /// <param name="defaultDirection">The default direction this terminal should face.</param>
        /// <param name="isCoalescing">Whether this terminal coalesces multiple input into a list.</param>
        public InputTerminalAttribute(Direction defaultDirection, bool isCoalescing = false) : base(defaultDirection)
        {
            IsCoalescing = isCoalescing;
        }

        /// <summary>
        /// Get or sets whether this terminal coalesces multiple input into a list.
        /// </summary>
        public bool IsCoalescing { get; set; }
    }
}