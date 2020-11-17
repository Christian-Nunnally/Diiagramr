using System;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// Attribute that can be applied to properties in implementations of <see cref="Node"/> to make those properties persisted through load/save.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NodeSettingAttribute : Attribute
    {
    }
}