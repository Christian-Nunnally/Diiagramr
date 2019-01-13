using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.Service;
using DiiagramrAPI.ViewModel.Diagram;
using Stylet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.ViewModel.ProjectScreen.Diagram
{
    public class TerminalViewModel : Screen
    {
        public const double TerminalDiameter = 2 * DiagramViewModel.NodeBorderWidth;
        public static TerminalViewModel SelectedTerminal;

        public static CornerRadius TerminalBorderCornerRadius = new CornerRadius(2);
        public static CornerRadius TerminalCornerRadius = new CornerRadius(3);
        public Action<object> DataChanged;
        private static readonly List<Action> ActionsToTakeWhenColorThemeIsLoaded = new List<Action>();
        private static ColorTheme _colorTheme;
        private readonly List<Action> ActionsToTakeWhenTypeIsLoaded = new List<Action>();
        private Adorner _adorner;
        private object _data;
        private bool _isSelected;

        public TerminalViewModel(TerminalModel terminal)
        {
            TerminalModel = terminal ?? throw new ArgumentNullException(nameof(terminal));
            TerminalModel.PropertyChanged += TerminalOnPropertyChanged;
            Data = TerminalModel.Data;
            Name = TerminalModel.Name;
            SetTerminalRotationBasedOnDirection();
            SetBackgroundBrushWhenColorThemeAndTypeLoad();
        }

        public event Action<TerminalViewModel, bool> WiringModeChanged;

        public static ColorTheme ColorTheme
        {
            get => _colorTheme;

            set
            {
                _colorTheme = value;
                if (_colorTheme == null)
                {
                    return;
                }

                foreach (var action in ActionsToTakeWhenColorThemeIsLoaded)
                {
                    action.Invoke();
                }

                ActionsToTakeWhenColorThemeIsLoaded.Clear();
            }
        }

        public Adorner Adorner
        {
            get => _adorner;

            set
            {
                if (value == null && _adorner != null)
                {
                    RemoveAllAdornersFromTerminal();
                }
                else if (value != null)
                {
                    AdornerLayer.GetAdornerLayer(View).Add(value);
                }
                _adorner = value;
            }
        }

        public virtual object Data
        {
            get => _data;

            set
            {
                _data = value;
                TerminalModel.Data = value;
                DataChanged?.Invoke(Data);
            }
        }

        public int EdgeIndex
        {
            get => TerminalModel.EdgeIndex;
            set => TerminalModel.EdgeIndex = value;
        }

        public virtual bool HighlightVisible { get; set; }
        public bool IsConnected => TerminalModel.ConnectedWires?.Any() ?? false;

        public virtual bool IsSelected
        {
            get => _isSelected;

            set
            {
                _isSelected = value;
                WiringModeChanged?.Invoke(this, _isSelected);

                if (SelectedTerminal == this)
                {
                    SelectedTerminal = null;
                    _isSelected = false;
                    return;
                }

                if (SelectedTerminal != null)
                {
                    if (SelectedTerminal.WireToTerminal(TerminalModel))
                    {
                        _isSelected = false;
                        return;
                    }
                }

                if (SelectedTerminal != null)
                {
                    SelectedTerminal.IsSelected = false;
                }

                SelectedTerminal = _isSelected ? this : null;
            }
        }

        public virtual bool MouseWithin { get; set; }

        public string Name { get; set; }

        public Brush TerminalBackgroundBrush { get; set; }

        public double TerminalDownWireMinimumLength
        {
            get => TerminalModel.TerminalDownWireMinimumLength;
            set => TerminalModel.TerminalDownWireMinimumLength = value;
        }

        public double TerminalLeftWireMinimumLength
        {
            get => TerminalModel.TerminalLeftWireMinimumLength;
            set => TerminalModel.TerminalLeftWireMinimumLength = value;
        }

        public virtual TerminalModel TerminalModel { get; }

        public double TerminalRightWireMinimumLength
        {
            get => TerminalModel.TerminalRightWireMinimumLength;
            set => TerminalModel.TerminalRightWireMinimumLength = value;
        }

        public float TerminalRotation { get; set; }

        public double TerminalUpWireMinimumLength
        {
            get => TerminalModel.TerminalUpWireMinimumLength;
            set => TerminalModel.TerminalUpWireMinimumLength = value;
        }

        public virtual bool TitleVisible => MouseWithin || IsSelected;

        public double XRelativeToNode
        {
            get => TerminalModel.OffsetX;
            set => TerminalModel.OffsetX = value;
        }

        public double YRelativeToNode
        {
            get => TerminalModel.OffsetY;
            set => TerminalModel.OffsetY = value;
        }

        public static TerminalViewModel CreateTerminalViewModel(TerminalModel terminal)
        {
            var terminalViewModel = terminal.Kind == TerminalKind.Input
                ? (TerminalViewModel)new InputTerminalViewModel(terminal)
                : (TerminalViewModel)new OutputTerminalViewModel(terminal);

            terminalViewModel.SetBackgroundBrushWhenColorThemeAndTypeLoad();
            return terminalViewModel;
        }

        public void CalculateUTurnLimitsForTerminal(double nodeWidth, double nodeHeight)
        {
            const double marginFromEdgeOfNode = DiagramViewModel.NodeBorderWidth + 10;
            var offsetX = TerminalModel.OffsetX;
            var offsetY = TerminalModel.OffsetY;
            var terminalDirection = TerminalModel.Direction;

            if (terminalDirection == Direction.North)
            {
                TerminalUpWireMinimumLength = 0;
                TerminalDownWireMinimumLength = nodeHeight + marginFromEdgeOfNode * 2;
                TerminalLeftWireMinimumLength = offsetX + marginFromEdgeOfNode;
                TerminalRightWireMinimumLength = nodeWidth - offsetX + marginFromEdgeOfNode;
            }
            else if (terminalDirection == Direction.South)
            {
                TerminalUpWireMinimumLength = nodeHeight + marginFromEdgeOfNode * 2;
                TerminalDownWireMinimumLength = 0;
                TerminalLeftWireMinimumLength = offsetX + marginFromEdgeOfNode;
                TerminalRightWireMinimumLength = nodeWidth - offsetX + marginFromEdgeOfNode;
            }
            else if (terminalDirection == Direction.East)
            {
                TerminalUpWireMinimumLength = offsetY + marginFromEdgeOfNode;
                TerminalDownWireMinimumLength = nodeHeight - offsetY + marginFromEdgeOfNode;
                TerminalLeftWireMinimumLength = nodeWidth + marginFromEdgeOfNode * 2;
                TerminalRightWireMinimumLength = 0;
            }
            else if (terminalDirection == Direction.West)
            {
                TerminalUpWireMinimumLength = offsetY + marginFromEdgeOfNode;
                TerminalDownWireMinimumLength = nodeHeight - offsetY + marginFromEdgeOfNode;
                TerminalLeftWireMinimumLength = 0;
                TerminalRightWireMinimumLength = nodeWidth + marginFromEdgeOfNode * 2;
            }
        }

        public virtual void DisconnectTerminal()
        {
            TerminalModel.DisconnectWires();
        }

        public void DropEventHandler(object sender, DragEventArgs e)
        {
            var o = e.Data.GetData(DataFormats.StringFormat);
            DropObject(o);
        }

        public virtual void DropObject(object o)
        {
            if (!(o is TerminalModel terminal))
            {
                return;
            }

            WireToTerminal(terminal);
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Adorner = new TerminalDataProbeAdorner(View, this);
            }
        }

        public void KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Adorner = null;
            }
        }

        public void LostFocus()
        {
            Adorner = null;
        }

        public void MouseEntered(object sender, MouseEventArgs e)
        {
            if (View != null)
            {
                Adorner = new ToolTipAdorner(View, this);
                View.Focusable = true;
                View.IsEnabled = true;
                View?.Focus();
            }
            MouseWithin = true;
        }

        public void MouseLeft(object sender, MouseEventArgs e)
        {
            MouseWithin = false;
            Adorner = null;
        }

        public void MouseMove(object sender, MouseEventArgs e)
        {
            //            var uiElement = (UIElement) sender;
            //            var dataObjectModel = new DataObject(DataFormats.StringFormat, TerminalModel);
            //            var dataObjectViewModel = new DataObject(DataFormats.StringFormat, this);
            //            if (e.LeftButton == MouseButtonState.Pressed)
            //                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            //                    DragDrop.DoDragDrop(uiElement, dataObjectViewModel, DragDropEffects.Link);
            //                else
            //                    DragDrop.DoDragDrop(uiElement, dataObjectModel, DragDropEffects.Link);
            e.Handled = true;
        }

        public virtual void SetTerminalDirection(Direction direction)
        {
            TerminalModel.Direction = direction;
        }

        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            HighlightVisible = TerminalModel.Type.IsAssignableFrom(type);
        }

        public void TerminalLeftMouseDown()
        {
            IsSelected = true;
        }

        public void TerminalLeftMouseDownHandler(object sender, MouseEventArgs e)
        {
            TerminalLeftMouseDown();
            e.Handled = true;
        }

        public virtual bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal == null)
            {
                return false;
            }

            if (terminal.Kind == TerminalModel.Kind)
            {
                return false;
            }

            if (terminal.ConnectedWires != null && terminal.ConnectedWires
                .Any(connectedWire => TerminalModel.ConnectedWires.Contains(connectedWire)))
            {
                return false;
            }

            new WireModel(TerminalModel, terminal);
            return true;
        }

        private void RemoveAllAdornersFromTerminal()
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(View);
            Adorner[] toRemoveArray = adornerLayer.GetAdorners(View);
            if (toRemoveArray != null)
            {
                for (int x = 0; x < toRemoveArray.Length; x++)
                {
                    adornerLayer.Remove(toRemoveArray[x]);
                }
            }
        }

        private void SetBackgroundBrushWhenColorThemeAndTypeLoad()
        {
            if (TerminalModel.Type == null)
            {
                ActionsToTakeWhenTypeIsLoaded.Add(SetBackgroundBrushWhenColorThemeLoads);
            }
            else
            {
                SetBackgroundBrushWhenColorThemeLoads();
            }
        }

        private void SetBackgroundBrushWhenColorThemeLoads()
        {
            if (ColorTheme == null)
            {
                ActionsToTakeWhenColorThemeIsLoaded.Add(() =>
                {
                    TerminalBackgroundBrush = new SolidColorBrush(ColorTheme.GetTerminalColorForType(TerminalModel.Type));
                });
            }
            else
            {
                TerminalBackgroundBrush = new SolidColorBrush(ColorTheme.GetTerminalColorForType(TerminalModel.Type));
            }
        }

        private void SetTerminalRotationBasedOnDirection()
        {
            switch (TerminalModel.Direction)
            {
                case Direction.North:
                    TerminalRotation = 0;
                    break;

                case Direction.East:
                    TerminalRotation = 90;
                    break;

                case Direction.South:
                    TerminalRotation = 180;
                    break;

                default:
                    TerminalRotation = 270;
                    break;
            }
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TerminalModel.Direction))
            {
                SetTerminalRotationBasedOnDirection();
            }
            else if (e.PropertyName == nameof(Model.TerminalModel.Data))
            {
                Data = TerminalModel.Data;
            }
            else if (e.PropertyName == nameof(Model.TerminalModel.Type))
            {
                if (TerminalModel.Type != null)
                {
                    ActionsToTakeWhenTypeIsLoaded.ForEach(x => x.Invoke());
                    ActionsToTakeWhenTypeIsLoaded.Clear();
                }
            }
        }
    }
}
