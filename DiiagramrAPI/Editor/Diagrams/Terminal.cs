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
    /// <summary>
    /// A view model for a terminal.
    /// </summary>
    public class Terminal : ViewModel<TerminalModel>, IMouseEnterLeaveReaction
    {
        /// <summary>
        /// The height of the terminal. This changes how much it protrudes out from the node.
        /// </summary>
        public const double TerminalHeight = 2 * Diagram.NodeBorderWidth;

        /// <summary>
        /// The width of the terminal.
        /// </summary>
        public const double TerminalWidth = TerminalHeight - 10;

        /// <summary>
        /// Creates a new terminal view model for the given model.
        /// </summary>
        /// <param name="terminal">The model to create the terminal view model from.</param>
        public Terminal(TerminalModel terminal)
        {
            Model = terminal ?? throw new ArgumentNullException(nameof(terminal));
            Model.PropertyChanged += TerminalOnPropertyChanged;
            Data = Model.Data;
            Name = Model.Name;
            SetTerminalRotationBasedOnDirection();
            SetTerminalColor();
        }

        /// <summary>
        /// The radius of the corner of the border or the terminal visual.
        /// </summary>
        public static CornerRadius TerminalBorderCornerRadius { get; } = new CornerRadius(2);

        /// <summary>
        /// The radius of the corner of the terminal visual.
        /// </summary>
        public static CornerRadius TerminalCornerRadius { get; } = new CornerRadius(3);

        /// <summary>
        /// The data value of the terminal.
        /// </summary>
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

        /// <summary>
        /// Gets or sets whether this terminal should be highlighted.
        /// </summary>
        public virtual bool HighlightVisible { get; set; }

        /// <summary>
        /// Gets whether this terminal is connected to any wires.
        /// </summary>
        public bool IsConnected => Model.ConnectedWires?.Any() ?? false;

        /// <summary>
        /// Gets or sets whether this terminal is selected.
        /// </summary>
        public virtual bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets the user visible name of this terminal.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the background brush of this terminal.
        /// </summary>
        public SolidColorBrush TerminalBackgroundBrush { get; set; }

        /// <summary>
        /// Gets or sets what the background brush of this terminal should be when the mouse is hovering over it.
        /// </summary>
        public SolidColorBrush TerminalBackgroundMouseOverBrush { get; set; }

        /// <summary>
        /// Gets or sets the about the terminal visual should be rotated based on its direction.
        /// </summary>
        public float TerminalRotation { get; set; }

        /// <summary>
        /// Gets or sets the X position of the terminal visual relative to the nodes view.
        /// </summary>
        public double ViewXPosition => XRelativeToNode - (TerminalWidth / 2);

        /// <summary>
        /// Gets or sets the Y position of the terminal visual relative to the nodes view.
        /// </summary>
        public double ViewYPosition => YRelativeToNode - (TerminalHeight / 2);

        /// <summary>
        /// Gets or sets the X position of the center of the terminal relative to the nodes view.
        /// </summary>
        public double XRelativeToNode
        {
            get => Model.OffsetX;
            set => Model.OffsetX = value;
        }

        /// <summary>
        /// Gets or sets the Y position of the center of the terminal relative to the nodes view.
        /// </summary>
        public double YRelativeToNode
        {
            get => Model.OffsetY;
            set => Model.OffsetY = value;
        }

        /// <summary>
        /// Factory method to create the correct terminal view model for a given terminal model.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public static Terminal CreateTerminalViewModel(TerminalModel terminal)
        {
            return terminal is InputTerminalModel inputTerminalModel
                ? (Terminal)new InputTerminal(inputTerminalModel)
                : new OutputTerminal((OutputTerminalModel)terminal);
        }

        /// <inheritdoc/>
        public void MouseEntered()
        {
            SetTerminalAdorner(new TerminalToolTipAdorner(View, this));
        }

        /// <inheritdoc/>
        public void MouseLeft()
        {
            if (!(Adorner is DirectEditAdorner))
            {
                SetTerminalAdorner(null);
            }
        }

        /// <summary>
        /// Shows the highlight on this terminal if the given type is compataible.
        /// </summary>
        /// <param name="type">The type to test.</param>
        public virtual void ShowHighlightIfCompatibleType(Type type)
        {
            if (!IsConnected)
            {
                HighlightVisible = ValueConverter.NonExaustiveCanConvertToType(type, Model.Type);
            }
        }

        /// <summary>
        /// Sets an adorner on this terminal.
        /// </summary>
        /// <param name="adorner">The adorner to set on the terminal.</param>
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