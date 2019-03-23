using System.Windows;
using System.Windows.Media;

namespace DiiagramrAPI.Diagram.Interactors
{
    public class TerminalWirer : DiagramInteractor
    {
        private const int GhostWireAlphaValue = 70;
        private SolidColorBrush _ghostWireBrush = new SolidColorBrush(Color.FromArgb(GhostWireAlphaValue, 128, 128, 128));
        private Wire _previewWire;
        private Terminal _wiringTerminal;

        public TerminalWirer()
        {
            Weight = 0.75;
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction.Diagram;
            var mousePosition = interaction.MousePosition;
            if (interaction.Type == InteractionType.MouseMoved) {
                _previewWire.X2 = diagram.GetDiagramPointFromViewPointX(mousePosition.X);
                _previewWire.Y2 = diagram.GetDiagramPointFromViewPointY(mousePosition.Y);

                if (interaction.ViewModelMouseIsOver is Terminal terminal && terminal != _wiringTerminal && terminal.HighlightVisible)
                {
                    _previewWire.X2 = terminal.Model.X + Terminal.TerminalDiameter / 2;
                    _previewWire.Y2 = terminal.Model.Y + Terminal.TerminalDiameter / 2;
                    var terminalColor = terminal.TerminalBackgroundBrush.Color;
                    var R = terminalColor.R;
                    var G = terminalColor.G;
                    var B = terminalColor.B;
                    _previewWire.LineColorBrush = new SolidColorBrush(Color.FromArgb(GhostWireAlphaValue, R, G, B));
                    _previewWire.BannedDirectionForEnd = DirectionHelpers.OppositeDirection(terminal.Model.Direction);
                }
                else
                {
                    _previewWire.LineColorBrush = _ghostWireBrush;
                    _previewWire.BannedDirectionForEnd = Direction.None;
                }
            }
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
                && interaction.ViewModelMouseIsOver is Terminal
                && !interaction.IsCtrlKeyPressed;
        }

        public override bool ShouldStopInteraction(DiagramInteractionEventArguments interaction)
        {
            return interaction.Type == InteractionType.RightMouseDown 
               || (interaction.Type == InteractionType.LeftMouseDown
                   && interaction.ViewModelMouseIsOver is Terminal terminal
                   && terminal != _wiringTerminal);
        }

        public override void StartInteraction(DiagramInteractionEventArguments interaction)
        {
            if (interaction.Type == InteractionType.LeftMouseDown && interaction.ViewModelMouseIsOver is Terminal terminal)
            {
                _wiringTerminal = terminal;
                StartWiringFromTerminal(interaction.Diagram, terminal, interaction.MousePosition);
            }
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            var type = interaction.Type;
            var elementMouseIsOver = interaction.ViewModelMouseIsOver;
            if (type == InteractionType.RightMouseDown)
            {
                CancelWireInsertion(interaction.Diagram);
            }
            else if (type == InteractionType.LeftMouseDown && elementMouseIsOver is Terminal terminal)
            {
                WireToTerminal(interaction.Diagram, terminal, interaction.MousePosition);
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

            var x1 = terminal.Model.X + Terminal.TerminalDiameter / 2;
            var y1 = terminal.Model.Y + Terminal.TerminalDiameter / 2;
            terminal.SetAdorner(null);
            _previewWire = new Wire(terminal, x1, y1);
            _previewWire.LineColorBrush = _ghostWireBrush;
            diagram.AddWire(_previewWire);
        }

        private void WireTerminalsToWiringTerminal(Diagram diagram, Terminal terminal)
        {
            _wiringTerminal.WireToTerminal(terminal.Model);
            diagram.RemoveWire(_previewWire);
            diagram.UnhighlightTerminals();
            diagram.UnselectTerminals();
            _previewWire = null;
            _wiringTerminal = null;
        }
    }
}
