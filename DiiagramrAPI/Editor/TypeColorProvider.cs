using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace DiiagramrAPI.Editor
{
    public class TypeColorProvider
    {
        public static readonly TypeColorProvider Instance = new TypeColorProvider();
        private readonly Color _defaultColor = Color.FromRgb(100, 100, 100);
        private readonly Dictionary<Type, Color> _terminalColorToTypeMap = new Dictionary<Type, Color>();

        private TypeColorProvider()
        {
            RegisterColorForType(typeof(int), Color.FromRgb(39, 116, 85));
            RegisterColorForType(typeof(float), Color.FromRgb(45, 110, 100));
            RegisterColorForType(typeof(byte[]), Color.FromRgb(170, 122, 57));
            RegisterColorForType(typeof(string), Color.FromRgb(153, 51, 80));
            RegisterColorForType(typeof(bool), Color.FromRgb(200, 51, 80));
            RegisterColorForType(typeof(object), Color.FromRgb(155, 155, 155));
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