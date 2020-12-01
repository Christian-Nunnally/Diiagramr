using System;

namespace DiiagramrAPI.Editor.Interactors
{
    /// <summary>
    /// Applied to implementations of <see cref="Node"/> to hide them from the node selector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HideFromNodeSelectorAttribute : Attribute
    {
    }
}