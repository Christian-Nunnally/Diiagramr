using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DiiagramrAPI.ViewModel.Diagram;

namespace DiiagramrAPI.Service
{
    public class ColorTheme
    {
        private readonly Dictionary<Type, Color> _wireColorToTypeMap = new Dictionary<Type, Color>();
        private readonly Dictionary<Type, Color> _terminalColorToTypeMap = new Dictionary<Type, Color>();

        private Color _defaultColor = Color.FromRgb(100, 100, 100);

        public ColorTheme()
        {
            RegisterWireColorForType(typeof(int), Color.FromRgb(73, 151, 120));
            RegisterWireColorForType(typeof(byte[]), Color.FromRgb(221, 173, 107));
            RegisterWireColorForType(typeof(string), Color.FromRgb(199, 97, 126));
            RegisterWireColorForType(typeof(object), Color.FromRgb(155, 155, 155));


            RegisterTerminalColorForType(typeof(int), Color.FromRgb(39, 116, 85));
            RegisterTerminalColorForType(typeof(byte[]), Color.FromRgb(170, 122, 57));
            RegisterTerminalColorForType(typeof(string), Color.FromRgb(153, 51, 80));
            RegisterTerminalColorForType(typeof(object), Color.FromRgb(155, 155, 155));
        }

        public Color GetWireColorForType(Type type)
        {
            return _wireColorToTypeMap.ContainsKey(type) ? _wireColorToTypeMap[type] : _defaultColor;
        }

        public void RegisterWireColorForType(Type type, Color color)
        {
            if (!_wireColorToTypeMap.ContainsKey(type)) _wireColorToTypeMap.Add(type, color);
        }

        public Color GetTerminalColorForType(Type type)
        {
            return _terminalColorToTypeMap.ContainsKey(type) ? _terminalColorToTypeMap[type] : _defaultColor;
        }

        public void RegisterTerminalColorForType(Type type, Color color)
        {
            if (!_terminalColorToTypeMap.ContainsKey(type)) _terminalColorToTypeMap.Add(type, color);
        }
    }
}
