using System.Drawing;

namespace DiiagramrAPI.Editor.Diagrams
{
    /// <summary>
    /// Interface implemented by custom types that can go across wires to customize the look of the type.
    /// </summary>
    public interface IWireableType
    {
        /// <summary>
        /// Gets the wire and terminal color for the type.
        /// </summary>
        /// <returns>The color shade for the type.</returns>
        Color GetTypeColor();
    }
}