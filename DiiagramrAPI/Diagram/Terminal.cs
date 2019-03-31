using DiiagramrAPI.Diagram.Interactors;
using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using DiiagramrAPI.Shell;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Diagram
{
    public class Terminal : ViewModel, IMouseEnterLeaveReaction
    {
        public const double TerminalHeight = 2 * Diagram.NodeBorderWidth;
        public const double TerminalWidth = TerminalHeight - 10;
        public static CornerRadius TerminalBorderCornerRadius = new CornerRadius(2);
        public static CornerRadius TerminalCornerRadius = new CornerRadius(3);
        public Action<object> DataChanged;
        private object _data;

        public Terminal(TerminalModel terminal)
        {
            Model = terminal ?? throw new ArgumentNullException(nameof(terminal));
            Model.PropertyChanged += TerminalOnPropertyChanged;
            Data = Model.Data;
            Name = Model.Name;
            SetTerminalRotationBasedOnDirection();
            SetTerminalColor();
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
        public string Name { get; set; }
        public SolidColorBrush TerminalBackgroundBrush { get; set; }
        public SolidColorBrush TerminalBackgroundMouseOverBrush { get; set; }

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

        public double ViewXPosition => XRelativeToNode - (Terminal.TerminalWidth / 2);

        public double ViewYPosition => YRelativeToNode - (Terminal.TerminalHeight / 2);

        public static Terminal CreateTerminalViewModel(TerminalModel terminal)
        {
            return terminal.Kind == TerminalKind.Input
                ? (Terminal)new InputTerminal(terminal)
                : (Terminal)new OutputTerminal(terminal);
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

        public virtual void SetTerminalDirection(Direction direction)
        {
            Model.Direction = direction;
        }

        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            if (!IsConnected)
            {
                HighlightVisible = Model.Type.IsAssignableFrom(type);
                NotifyOfPropertyChange(nameof(HighlightVisible));
            }
        }

        private void SetTerminalColor()
        {
            var color = TypeColorProvider.Instance.GetColorForType(Model.Type);
            TerminalBackgroundBrush = new SolidColorBrush(color);
            TerminalBackgroundMouseOverBrush = new SolidColorBrush(CoreUilities.ChangeColorBrightness(color, 0.5f));
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
            else if (e.PropertyName == nameof(TerminalModel.Data))
            {
                Data = Model.Data;
            }
            else if (e.PropertyName == nameof(TerminalModel.Type))
            {
                SetTerminalColor();
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

        public void MouseEntered()
        {
            SetTerminalAdorner(new TerminalToolTipAdorner(View, this));
        }

        public void MouseLeft()
        {
            if (!(Adorner is DirectEditTextBoxAdorner))
            {
                SetTerminalAdorner(null);
            }
        }
    }
}
