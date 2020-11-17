using System;
using System.Collections.Generic;
using System.Drawing;

namespace DiiagramrAPI.Editor
{
    /// <summary>
    /// Provides colors for types.
    /// </summary>
    public class TypeColorProvider
    {
        /// <summary>
        /// A static instance of <see cref="TypeColorProvider"/> everyone can use.
        /// </summary>
        public static readonly TypeColorProvider Instance = new TypeColorProvider();

        private readonly Color _defaultColor = Color.FromArgb(255, 100, 100, 100);
        private readonly Dictionary<Type, Color> _terminalColorToTypeMap = new Dictionary<Type, Color>();

        /// <summary>
        /// Creates a new instance of <see cref="TypeColorProvider"/>.
        /// </summary>
        private TypeColorProvider()
        {
            RegisterColorForType(typeof(int), Color.FromArgb(255, 45, 110, 100));
            RegisterColorForType(typeof(float), Color.FromArgb(255, 45, 110, 100));
            RegisterColorForType(typeof(double), Color.FromArgb(255, 45, 110, 100));
            RegisterColorForType(typeof(byte), Color.FromArgb(255, 45, 110, 100));

            RegisterColorForType(typeof(int[]), ColorTranslator.FromHtml("#545893"));
            RegisterColorForType(typeof(float[]), ColorTranslator.FromHtml("#545893"));
            RegisterColorForType(typeof(double[]), ColorTranslator.FromHtml("#545893"));
            RegisterColorForType(typeof(byte[]), ColorTranslator.FromHtml("#545893"));

            RegisterColorForType(typeof(string), Color.FromArgb(255, 153, 51, 80));
            RegisterColorForType(typeof(bool), Color.FromArgb(255, 200, 51, 80));
            RegisterColorForType(typeof(object), Color.FromArgb(255, 155, 155, 155));
        }

        /// <summary>
        /// Gets a color for a type.
        /// </summary>
        /// <param name="type">The type to get the color of.</param>
        /// <returns>The color for the type.</returns>
        public Color GetColorForType(Type type)
        {
            if (type == null)
            {
                return _defaultColor;
            }

            return _terminalColorToTypeMap.ContainsKey(type) ? _terminalColorToTypeMap[type] : _defaultColor;
        }

        /// <summary>
        /// Registeres a type with a color.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="color">The types color.</param>
        public void RegisterColorForType(Type type, Color color)
        {
            if (!_terminalColorToTypeMap.ContainsKey(type))
            {
                _terminalColorToTypeMap.Add(type, color);
            }
        }
    }
}