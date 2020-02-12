using System;
using System.Collections.Generic;
using System.Drawing;

namespace DiiagramrAPI.Editor
{
    public class TypeColorProvider
    {
        public static readonly TypeColorProvider Instance = new TypeColorProvider();
        private readonly Color _defaultColor = Color.FromArgb(255, 100, 100, 100);
        private readonly Dictionary<Type, Color> _terminalColorToTypeMap = new Dictionary<Type, Color>();

        private TypeColorProvider()
        {
            RegisterColorForType(typeof(int), Color.FromArgb(255, 39, 116, 85));
            RegisterColorForType(typeof(float), Color.FromArgb(255, 45, 110, 100));
            RegisterColorForType(typeof(byte[]), Color.FromArgb(255, 170, 122, 57));
            RegisterColorForType(typeof(string), Color.FromArgb(255, 153, 51, 80));
            RegisterColorForType(typeof(bool), Color.FromArgb(255, 200, 51, 80));
            RegisterColorForType(typeof(object), Color.FromArgb(255, 155, 155, 155));
            RegisterColorForType(typeof(float[]), ColorTranslator.FromHtml("#545893"));
        }

        public Color GetColorForType(Type type)
        {
            if (type == null)
            {
                return _defaultColor;
            }

            return _terminalColorToTypeMap.ContainsKey(type) ? _terminalColorToTypeMap[type] : _defaultColor;
        }

        public void RegisterColorForType(Type type, Color color)
        {
            if (!_terminalColorToTypeMap.ContainsKey(type))
            {
                _terminalColorToTypeMap.Add(type, color);
            }
        }
    }
}