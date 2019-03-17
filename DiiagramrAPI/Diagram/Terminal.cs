using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DiiagramrAPI.Diagram
{
    public class Terminal : ViewModel
    {
        public const double TerminalDiameter = 2 * Diagram.NodeBorderWidth;
        public static CornerRadius TerminalBorderCornerRadius = new CornerRadius(2);
        public static CornerRadius TerminalCornerRadius = new CornerRadius(3);
        public Action<object> DataChanged;
        private static readonly List<Action> ActionsToTakeWhenColorThemeIsLoaded = new List<Action>();
        private static ColorTheme _colorTheme;
        private readonly List<Action> ActionsToTakeWhenTypeIsLoaded = new List<Action>();
        private object _data;

        public Terminal(TerminalModel terminal)
        {
            Model = terminal ?? throw new ArgumentNullException(nameof(terminal));
            Model.PropertyChanged += TerminalOnPropertyChanged;
            Data = Model.Data;
            Name = Model.Name;
            SetTerminalRotationBasedOnDirection();
            SetBackgroundBrushWhenColorThemeAndTypeLoad();
        }

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

        public virtual object Data
        {
            get => _data;

            set
            {
                _data = value;
                Model.Data = value;
                DataChanged?.Invoke(Data);
            }
        }

        public int EdgeIndex
        {
            get => Model.EdgeIndex;
            set => Model.EdgeIndex = value;
        }

        public virtual bool HighlightVisible { get; set; }
        public bool IsConnected => Model.ConnectedWires?.Any() ?? false;
        public virtual bool IsSelected { get; set; }
        public virtual bool MouseWithin { get; set; }
        public string Name { get; set; }
        public Brush TerminalBackgroundBrush { get; set; }

        public double TerminalDownWireMinimumLength
        {
            get => Model.TerminalDownWireMinimumLength;
            set => Model.TerminalDownWireMinimumLength = value;
        }

        public double TerminalLeftWireMinimumLength
        {
            get => Model.TerminalLeftWireMinimumLength;
            set => Model.TerminalLeftWireMinimumLength = value;
        }

        public virtual TerminalModel Model { get; }

        public double TerminalRightWireMinimumLength
        {
            get => Model.TerminalRightWireMinimumLength;
            set => Model.TerminalRightWireMinimumLength = value;
        }

        public float TerminalRotation { get; set; }

        public double TerminalUpWireMinimumLength
        {
            get => Model.TerminalUpWireMinimumLength;
            set => Model.TerminalUpWireMinimumLength = value;
        }

        public virtual bool TitleVisible => MouseWithin || IsSelected;

        public double XRelativeToNode
        {
            get => Model.OffsetX;
            set => Model.OffsetX = value;
        }

        public double YRelativeToNode
        {
            get => Model.OffsetY;
            set => Model.OffsetY = value;
        }

        public static Terminal CreateTerminalViewModel(TerminalModel terminal)
        {
            var terminalViewModel = terminal.Kind == TerminalKind.Input
                ? (Terminal)new InputTerminal(terminal)
                : (Terminal)new OutputTerminal(terminal);

            terminalViewModel.SetBackgroundBrushWhenColorThemeAndTypeLoad();
            return terminalViewModel;
        }

        public void CalculateUTurnLimitsForTerminal(double nodeWidth, double nodeHeight)
        {
            const double marginFromEdgeOfNode = Diagram.NodeBorderWidth + 10;
            var offsetX = Model.OffsetX;
            var offsetY = Model.OffsetY;
            var terminalDirection = Model.Direction;

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
            Model.DisconnectWires();
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

        public void LostFocus()
        {
            SetTerminalAdorner(null);
        }

        public void MouseEntered(object sender, MouseEventArgs e)
        {
            if (View != null)
            {
                if (!IsSelected)
                {
                    SetTerminalAdorner(new TerminalToolTipAdorner(View, this));
                    View.Focusable = true;
                    View.IsEnabled = true;
                    View?.Focus();
                }
            }
            MouseWithin = true;
        }

        public void MouseLeft(object sender, MouseEventArgs e)
        {
            MouseWithin = false;
            if (!(Adorner is DirectEditTextBoxAdorner))
            {
                SetTerminalAdorner(null);
            }
        }

        public virtual void SetTerminalDirection(Direction direction)
        {
            Model.Direction = direction;
        }

        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            HighlightVisible = Model.Type.IsAssignableFrom(type);
        }

        public virtual bool WireToTerminal(TerminalModel terminal)
        {
            if (terminal == null)
            {
                return false;
            }

            if (terminal.Kind == Model.Kind)
            {
                return false;
            }

            if (terminal.ConnectedWires != null && terminal.ConnectedWires
                .Any(connectedWire => Model.ConnectedWires.Contains(connectedWire)))
            {
                return false;
            }

            new WireModel(Model, terminal);
            return true;
        }

        private void SetBackgroundBrushWhenColorThemeAndTypeLoad()
        {
            if (Model.Type == null)
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
                    TerminalBackgroundBrush = new SolidColorBrush(ColorTheme.GetTerminalColorForType(Model.Type));
                });
            }
            else
            {
                TerminalBackgroundBrush = new SolidColorBrush(ColorTheme.GetTerminalColorForType(Model.Type));
            }
        }

        private void SetTerminalRotationBasedOnDirection()
        {
            switch (Model.Direction)
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
            if (e.PropertyName == nameof(Model.Direction))
            {
                SetTerminalRotationBasedOnDirection();
            }
            else if (e.PropertyName == nameof(DiiagramrAPI.Diagram.Model.TerminalModel.Data))
            {
                Data = Model.Data;
            }
            else if (e.PropertyName == nameof(DiiagramrAPI.Diagram.Model.TerminalModel.Type))
            {
                if (Model.Type != null)
                {
                    ActionsToTakeWhenTypeIsLoaded.ForEach(x => x.Invoke());
                    ActionsToTakeWhenTypeIsLoaded.Clear();
                }
            }
        }

        protected void SetTerminalAdorner(Adorner adorner)
        {
            if (adorner is DirectEditTextBoxAdorner)
            {
                SetAdorner(adorner);
            }
            else if (Adorner == null)
            {
                SetAdorner(adorner);
            }
            else if (adorner == null)
            {
                SetAdorner(adorner);
            }
        }
    }
}
