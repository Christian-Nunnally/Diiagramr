using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Diiagramr.Model;
using Diiagramr.Service;
using Diiagramr.ViewModel.Diagram;
using PropertyChanged;
using Stylet;

namespace Diiagramr.PluginNodeApi
{
    [Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West
    }
}