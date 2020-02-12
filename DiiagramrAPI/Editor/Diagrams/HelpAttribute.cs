using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    [AttributeUsage(AttributeTargets.All)]
    public class HelpAttribute : Attribute
    {
        public HelpAttribute(string helpText)
        {
            HelpText = helpText;
        }

        public string HelpText { get; }
    }
}