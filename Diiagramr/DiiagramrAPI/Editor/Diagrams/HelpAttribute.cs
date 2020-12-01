using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// An attribute that can be applied to classes and terminal to provide built in help in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class HelpAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="HelpAttribute"/>
        /// </summary>
        /// <param name="helpText">The help to display in the editor.</param>
        public HelpAttribute(string helpText)
        {
            HelpText = helpText;
        }

        /// <summary>
        /// The help to display in the editor.
        /// </summary>
        public string HelpText { get; }
    }
}