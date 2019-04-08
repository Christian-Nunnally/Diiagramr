using DiiagramrAPI.Diagram.Model;
using DiiagramrAPI.Service;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class TerminalWirer : DiagramInteractor
    {
        private const int GhostWireAlphaValue = 70;
        private readonly SolidColorBrush _ghostWireBrush = new SolidColorBrush(Color.FromArgb(GhostWireAlphaValue, 128, 128, 128));
        private Wire _previewWire;
        private Terminal _wiringTerminal;
        private int _leftMouseDownCount;

        public TerminalWirer()
        {
            Weight = 0.75;
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            var elementUnderMouse = interaction.ViewModelUnderMouse;
            if (interaction.Type == InteractionType.MouseMoved)
            {
                ProcessMouseMove(diagram, mousePosition, elementUnderMouse);
            }
            else if (interaction.Type == InteractionType.LeftMouseDown)
            {
                ProcessLeftMouseDown(diagram, elementUnderMouse);
            }
        }

        private void ProcessLeftMouseDown(Diagram diagram, Stylet.Screen elementUnderMouse)
        {
            _leftMouseDownCount++;
            if (_leftMouseDownCount == 2)
            {
                if (elementUnderMouse is Terminal terminal && terminal == _wiringTerminal)
                {
                    ShowDirectEditTextboxOnTerminal(diagram, terminal);
                }
            }
        }

        private void ShowDirectEditTextboxOnTerminal(Diagram diagram, Terminal terminal)
        {
            CancelWireInsertion(diagram);
            var directEditTextbox = new DirectEditTextBoxAdorner(terminal.View, terminal);
            if (directEditTextbox.IsDirectlyEditableType)
            {
                terminal.SetAdorner(directEditTextbox);
            }
        }

        private void ProcessMouseMove(Diagram diagram, Point mousePosition, Stylet.Screen elementUnderMouse)
        {
            SetPreviewWireEndPosition(diagram, mousePosition);
            if (elementUnderMouse is Terminal terminal && terminal.HighlightVisible && terminal != _wiringTerminal)
            {
                PreviewConnectingWireToTerminal(terminal);
            }
            else
            {
                _previewWire.LineColorBrush = _ghostWireBrush;
                _previewWire.BannedDirectionForStart = Direction.None;
            }
        }

        private void SetPreviewWireEndPosition(Diagram diagram, Point mousePosition)
        {
            _previewWire.X1 = diagram.GetDiagramPointFromViewPointX(mousePosition.X);
            _previewWire.Y1 = diagram.GetDiagramPointFromViewPointY(mousePosition.Y);
        }

        private void PreviewConnectingWireToTerminal(Terminal terminal)
        {
            _previewWire.X1 = terminal.Model.X;
            _previewWire.Y1 = terminal.Model.Y;
            var terminalColor = terminal.TerminalBackgroundBrush.Color;
            var R = terminalColor.R;
            var G = terminalColor.G;
            var B = terminalColor.B;
            _previewWire.LineColorBrush = new SolidColorBrush(Color.FromArgb(GhostWireAlphaValue, R, G, B));
            _previewWire.BannedDirectionForStart = DirectionHelpers.OppositeDirection(terminal.Model.Direction);
        }

        private void CancelWireInsertion(Diagram diagram)
        {
            diagram.UnhighlightTerminals();
            diagram.UnselectNodes();
            diagram.RemoveWire(_previewWire);
        }

        public override bool ShouldStartInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.LeftMouseDown
                && interaction.ViewModelUnderMouse is Terminal
                && !interaction.IsCtrlKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown
               || (interaction.Type == InteractionType.LeftMouseDown
                   && interaction.ViewModelUnderMouse is Terminal terminal
                   && (terminal != _wiringTerminal || _leftMouseDownCount == 2));
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.LeftMouseDown && interaction.ViewModelUnderMouse is Terminal terminal)
            {
                _wiringTerminal = terminal;
                StartWiringFromTerminal(interaction.Diagram, terminal, interaction.MousePosition);
                _leftMouseDownCount = 0;
            }
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            var type = interaction.Type;
            var diagram = interaction.Diagram;
            var elementUnderMouse = interaction.ViewModelUnderMouse;
            var mousePosition = interaction.MousePosition;
            if (type == InteractionType.RightMouseDown)
            {
                CancelWireInsertion(interaction.Diagram);
            }
            else if (type == InteractionType.LeftMouseDown && elementUnderMouse is Terminal terminal)
            {
                WireToTerminal(diagram, terminal, mousePosition);
            }
        }

        private void WireToTerminal(Diagram diagram, Terminal terminal, Point mousePosition)
        {
            if (_wiringTerminal == terminal)
            {
                CancelWireInsertion(diagram);
                return;
            }
            WireTerminalsToWiringTerminal(diagram, terminal);
        }

        private void StartWiringFromTerminal(Diagram diagram, Terminal terminal, Point mousePosition)
        {
            diagram.UnselectTerminals();
            diagram.UnselectNodes();
            diagram.HighlightTerminalsOfSameType(terminal.Model);

            var x1 = terminal.Model.X;
            var y1 = terminal.Model.Y;
            terminal.SetAdorner(null);
            _previewWire = new Wire(terminal, x1, y1)
            {
                LineColorBrush = _ghostWireBrush
            };
            diagram.AddWire(_previewWire);
        }

        private void WireTerminalsToWiringTerminal(Diagram diagram, Terminal terminal)
        {
            TryWireTwoTerminalsOnDiagram(diagram, _wiringTerminal, terminal, true);
            diagram.RemoveWire(_previewWire);
            diagram.UnhighlightTerminals();
            diagram.UnselectTerminals();
            _previewWire = null;
            _wiringTerminal = null;
        }

        public static void TryWireTwoTerminalsOnDiagram(Diagram diagram, Terminal startTerminal, Terminal endTerminal, bool animateWire)
        {
            if (CanWireTwoTerminalsOnDiagram(diagram, startTerminal, endTerminal))
            {
                WireTwoTerminalsOnDiagram(diagram, startTerminal, endTerminal, animateWire);
            }
        }

        private static void WireTwoTerminalsOnDiagram(Diagram diagram, Terminal startTerminal, Terminal endTerminal, bool animateWire)
        {
            var wireModel = new WireModel(startTerminal.Model, endTerminal.Model) { };
            var wire = new Wire(wireModel)
            {
                DoAnimationWhenViewIsLoaded = animateWire
            };
            diagram.AddWire(wire);
        }

        public static bool CanWireTwoTerminalsOnDiagram(Diagram diagram, Terminal startTerminal, Terminal endTerminal)
        {
            if (endTerminal?.Model == null)
            {
                return false;
            }
            if (endTerminal.Model.Kind == startTerminal.Model.Kind)
            {
                return false;
            }
            if (endTerminal.Model.ConnectedWires.Any(connectedWire => startTerminal.Model.ConnectedWires.Contains(connectedWire)))
            {
                return false;
            }

            var sinkTerminal = startTerminal.Model.Kind == TerminalKind.Input ? startTerminal.Model : endTerminal.Model;
            var sourceTerminal = startTerminal.Model.Kind == TerminalKind.Output ? startTerminal.Model : endTerminal.Model;

            if (!sourceTerminal.Type.IsSubclassOf(sinkTerminal.Type) && sourceTerminal.Type != sinkTerminal.Type)
            {
                if (sourceTerminal.Type != typeof(object))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
