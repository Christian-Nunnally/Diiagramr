using DiiagramrAPI.Application;
using DiiagramrAPI.Editor.Interactors;
using DiiagramrCore;
using DiiagramrModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Diagrams
{
    public class Terminal : ViewModel, IMouseEnterLeaveReaction
    {
        public const double TerminalHeight = 2 * Diagram.NodeBorderWidth;
        public const double TerminalWidth = TerminalHeight - 10;

        public Terminal(TerminalModel terminal)
        {
            Model = terminal ?? throw new ArgumentNullException(nameof(terminal));
            Model.PropertyChanged += TerminalOnPropertyChanged;
            Data = Model.Data;
            Name = Model.Name;
            SetTerminalRotationBasedOnDirection();
            SetTerminalColor();
        }

        public static CornerRadius TerminalBorderCornerRadius { get; } = new CornerRadius(2);

        public static CornerRadius TerminalCornerRadius { get; } = new CornerRadius(3);

        public virtual object Data
        {
            get => Model.Data;

            set
            {
                if (Model.Data != value)
                {
                    Model.Data = value;
                }
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

        public virtual TerminalModel Model { get; }

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

        public double ViewXPosition => XRelativeToNode - (TerminalWidth / 2);

        public double ViewYPosition => YRelativeToNode - (TerminalHeight / 2);

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
            return terminal is InputTerminalModel inputTerminalModel
                ? (Terminal)new InputTerminal(inputTerminalModel)
                : new OutputTerminal((OutputTerminalModel)terminal);
        }

        public void CalculateUTurnLimitsForTerminal(double nodeWidth, double nodeHeight)
        {
            const double marginFromEdgeOfNode = Diagram.NodeBorderWidth + 10;
            var offsetX = Model.OffsetX;
            var offsetY = Model.OffsetY;
            var halfTerminalHeight = (TerminalHeight / 2);
            var terminalDirection = Model.DefaultSide;

            if (terminalDirection == Direction.North)
            {
                TerminalUpWireMinimumLength = 0;
                TerminalDownWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeHeight;
                TerminalLeftWireMinimumLength = marginFromEdgeOfNode + offsetX - halfTerminalHeight;
                TerminalRightWireMinimumLength = marginFromEdgeOfNode + (nodeWidth - offsetX) + halfTerminalHeight;
            }
            else if (terminalDirection == Direction.South)
            {
                TerminalUpWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeHeight;
                TerminalDownWireMinimumLength = marginFromEdgeOfNode + 0;
                TerminalLeftWireMinimumLength = marginFromEdgeOfNode + offsetX - halfTerminalHeight;
                TerminalRightWireMinimumLength = marginFromEdgeOfNode + (nodeWidth - offsetX) + halfTerminalHeight;
            }
            else if (terminalDirection == Direction.East)
            {
                TerminalUpWireMinimumLength = marginFromEdgeOfNode + offsetY - halfTerminalHeight;
                TerminalDownWireMinimumLength = marginFromEdgeOfNode + (nodeHeight - offsetY) + halfTerminalHeight;
                TerminalLeftWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeWidth;
                TerminalRightWireMinimumLength = marginFromEdgeOfNode + 0;
            }
            else if (terminalDirection == Direction.West)
            {
                TerminalUpWireMinimumLength = marginFromEdgeOfNode + offsetY - halfTerminalHeight;
                TerminalDownWireMinimumLength = marginFromEdgeOfNode + (nodeHeight - offsetY) + halfTerminalHeight;
                TerminalLeftWireMinimumLength = marginFromEdgeOfNode + 0;
                TerminalRightWireMinimumLength = marginFromEdgeOfNode + marginFromEdgeOfNode + nodeWidth;
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

        public virtual void SetTerminalDirection(Direction direction)
        {
            Model.DefaultSide = direction;
        }

        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            if (!IsConnected)
            {
                HighlightVisible = Model.CanWireToType(type) || Model.CanWireFromType(type);
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

        private static SolidColorBrush GetBrushFromColor(System.Drawing.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        private void SetTerminalColor()
        {
            var color = TypeColorProvider.Instance.GetColorForType(Model.Type);
            TerminalBackgroundBrush = GetBrushFromColor(color);
            TerminalBackgroundMouseOverBrush = GetBrushFromColor(CoreUilities.ChangeColorBrightness(color, 0.5f));
        }

        private void SetTerminalRotationBasedOnDirection()
        {
            TerminalRotation = Model.DefaultSide switch
            {
                Direction.North => 0,
                Direction.East => 90,
                Direction.South => 180,
                _ => 270,
            };
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.DefaultSide))
            {
                SetTerminalRotationBasedOnDirection();
            }
            else if (e.PropertyName == nameof(TerminalModel.Data))
            {
                NotifyOfPropertyChange(nameof(Data));
            }
            else if (e.PropertyName == nameof(TerminalModel.Type))
            {
                SetTerminalColor();
            }
        }
    }
}