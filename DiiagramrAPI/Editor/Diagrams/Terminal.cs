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
            TerminalModel = Model as TerminalModel;
            TerminalModel.PropertyChanged += TerminalOnPropertyChanged;
            Data = TerminalModel.Data;
            Name = TerminalModel.Name;
            SetTerminalRotationBasedOnDirection();
            SetTerminalColor();
        }

        public static CornerRadius TerminalBorderCornerRadius { get; } = new CornerRadius(2);

        public static CornerRadius TerminalCornerRadius { get; } = new CornerRadius(3);

        public virtual object Data
        {
            get => TerminalModel.Data;

            set
            {
                if (TerminalModel.Data != value)
                {
                    TerminalModel.Data = value;
                }
            }
        }

        public int EdgeIndex
        {
            get => TerminalModel.EdgeIndex;
            set => TerminalModel.EdgeIndex = value;
        }

        public virtual bool HighlightVisible { get; set; }

        public bool IsConnected => TerminalModel.ConnectedWires?.Any() ?? false;

        public virtual bool IsSelected { get; set; }

        public virtual TerminalModel TerminalModel { get; }

        public string Name { get; set; }

        public SolidColorBrush TerminalBackgroundBrush { get; set; }

        public SolidColorBrush TerminalBackgroundMouseOverBrush { get; set; }

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

        public double ViewXPosition => XRelativeToNode - (TerminalWidth / 2);

        public double ViewYPosition => YRelativeToNode - (TerminalHeight / 2);

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

        public static Terminal CreateTerminalViewModel(TerminalModel terminal)
        {
            return terminal is InputTerminalModel inputTerminalModel
                ? (Terminal)new InputTerminal(inputTerminalModel)
                : new OutputTerminal((OutputTerminalModel)terminal);
        }

        public void CalculateUTurnLimitsForTerminal(double nodeWidth, double nodeHeight)
        {
            const double marginFromEdgeOfNode = Diagram.NodeBorderWidth + 10;
            var offsetX = TerminalModel.OffsetX;
            var offsetY = TerminalModel.OffsetY;
            var halfTerminalHeight = (TerminalHeight / 2);
            var terminalDirection = TerminalModel.DefaultSide;

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
            TerminalModel.DefaultSide = direction;
        }

        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            if (!IsConnected)
            {
                HighlightVisible = ValueConverter.NonExaustiveCanConvertToType(type, TerminalModel.Type);
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
            var color = TypeColorProvider.Instance.GetColorForType(TerminalModel.Type);
            TerminalBackgroundBrush = GetBrushFromColor(color);
            TerminalBackgroundMouseOverBrush = GetBrushFromColor(CoreUilities.ChangeColorBrightness(color, 0.5f));
        }

        private void SetTerminalRotationBasedOnDirection()
        {
            TerminalRotation = TerminalModel.DefaultSide switch
            {
                Direction.North => 0,
                Direction.East => 90,
                Direction.South => 180,
                _ => 270,
            };
        }

        private void TerminalOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TerminalModel.DefaultSide))
            {
                SetTerminalRotationBasedOnDirection();
            }
            else if (e.PropertyName == nameof(DiiagramrModel.TerminalModel.Data))
            {
                base.NotifyOfPropertyChange(nameof(Data));
            }
            else if (e.PropertyName == nameof(DiiagramrModel.TerminalModel.Type))
            {
                SetTerminalColor();
            }
        }
    }
}