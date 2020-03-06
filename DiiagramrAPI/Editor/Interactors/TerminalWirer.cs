using DiiagramrAPI.Application.Commands.Transacting;
using DiiagramrAPI.Commands;
using DiiagramrAPI.Editor.Diagrams;
using DiiagramrModel;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DiiagramrAPI.Editor.Interactors
{
    public class TerminalWirer : DiagramInteractor
    {
        private const int GhostWireAlphaValue = 70;
        private readonly SolidColorBrush _ghostWireBrush = new SolidColorBrush(Color.FromArgb(GhostWireAlphaValue, 128, 128, 128));
        private readonly ITransactor _transactor;
        private readonly WirePathingAlgorithum _wirePathingAlgorithum = new WirePathingAlgorithum();
        private int _leftMouseDownCount;
        private TerminalModel _previewTerminal;
        private Wire _previewWire;
        private Terminal _wiringTerminal;

        public TerminalWirer(Func<ITransactor> transactorFactory)
        {
            _transactor = transactorFactory?.Invoke() ?? throw new ArgumentNullException(nameof(transactorFactory));
            Weight = 0.75;
        }

        public static bool CanWireTwoTerminalsOnDiagram(Terminal startTerminal, Terminal endTerminal)
        {
            if (endTerminal?.Model == null)
            {
                return false;
            }

            if (endTerminal.Model.GetType() == startTerminal.Model.GetType())
            {
                return false;
            }

            if (endTerminal.Model.ConnectedWires.Any(connectedWire => startTerminal.Model.ConnectedWires.Contains(connectedWire)))
            {
                return false;
            }

            var sinkTerminal = startTerminal.Model is InputTerminalModel ? startTerminal.Model : endTerminal.Model;
            var sourceTerminal = startTerminal.Model is OutputTerminalModel ? startTerminal.Model : endTerminal.Model;
            return sourceTerminal.CanWireToType(sinkTerminal.Type);
        }

        public static void TryWireTwoTerminalsOnDiagram(Diagram diagram, Terminal startTerminal, Terminal endTerminal, ITransactor transactor, bool animateWire)
        {
            if (CanWireTwoTerminalsOnDiagram(startTerminal, endTerminal))
            {
                WireTwoTerminalsOnDiagram(diagram, startTerminal, endTerminal, transactor, animateWire);
            }
        }

        public override void ProcessInteraction(DiagramInteractionEventArguments interaction)
        {
            var diagram = interaction?.Diagram ?? throw new ArgumentNullException(nameof(interaction));
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
                StartWiringFromTerminal(interaction.Diagram, terminal);
                _leftMouseDownCount = 0;
            }
        }

        public override void StopInteraction(DiagramInteractionEventArguments interaction)
        {
            var type = interaction.Type;
            var diagram = interaction.Diagram;
            var elementUnderMouse = interaction.ViewModelUnderMouse;
            if (type == InteractionType.RightMouseDown)
            {
                CancelWireInsertion(interaction.Diagram);
            }
            else if (type == InteractionType.LeftMouseDown && elementUnderMouse is Terminal terminal)
            {
                WireToTerminal(diagram, terminal);
            }
        }

        private static void WireTwoTerminalsOnDiagram(Diagram diagram, Terminal startTerminal, Terminal endTerminal, ITransactor transactor, bool animateWire)
        {
            var wireToTerminalCommand = new WireToTerminalCommand(diagram, startTerminal.Model, animateWire);
            transactor.Transact(wireToTerminalCommand, endTerminal.Model);
        }

        private void CancelWireInsertion(Diagram diagram)
        {
            diagram.UnhighlightTerminals();
            diagram.UnselectNodes();
            diagram.RemoveWire(_previewWire);
        }

        private void PreviewConnectingWireToTerminal(Terminal terminal)
        {
            var terminalColor = terminal.TerminalBackgroundBrush.Color;
            var r = terminalColor.R;
            var g = terminalColor.G;
            var b = terminalColor.B;
            _previewWire.LineColorBrush = new SolidColorBrush(Color.FromArgb(GhostWireAlphaValue, r, g, b));
            _wirePathingAlgorithum.EnableUTurnLimitsForSinkTerminal = true;
            _wirePathingAlgorithum.WireDistanceOutOfSinkTerminal = 25.0f;
            _previewWire.WireModel.SinkTerminal = terminal.Model;
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

        private void ProcessMouseMove(Diagram diagram, Point mousePosition, Stylet.Screen elementUnderMouse)
        {
            SetPreviewWireEndPosition(diagram, mousePosition);
            if (elementUnderMouse is Terminal terminal && terminal.HighlightVisible && terminal != _wiringTerminal)
            {
                PreviewConnectingWireToTerminal(terminal);
            }
            else
            {
                _wirePathingAlgorithum.EnableUTurnLimitsForSinkTerminal = false;
                _wirePathingAlgorithum.WireDistanceOutOfSinkTerminal = 0;
                _previewWire.WireModel.SinkTerminal = _previewTerminal;
                _previewWire.LineColorBrush = _ghostWireBrush;
            }
        }

        private void SetPreviewWireEndPosition(Diagram diagram, Point mousePosition)
        {
            _previewTerminal.OffsetX = diagram.GetDiagramPointFromViewPointX(mousePosition.X);
            _previewTerminal.OffsetY = diagram.GetDiagramPointFromViewPointY(mousePosition.Y);
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

        private void StartWiringFromTerminal(Diagram diagram, Terminal terminal)
        {
            diagram.UnselectTerminals();
            diagram.UnselectNodes();
            diagram.HighlightWirableTerminals(terminal.Model);

            var x1 = terminal.Model.X;
            var y1 = terminal.Model.Y;
            terminal.SetAdorner(null);

            _previewTerminal = new TerminalModel("invsiblePreviewTerminal", typeof(object), terminal.Model.DefaultSide)
            {
                OffsetX = x1,
                OffsetY = y1
            };
            // TODO: Make this a different wire type maybe?
            var previewWireModel = new WireModel()
            {
                SourceTerminal = terminal.Model,
                SinkTerminal = _previewTerminal,
                DisableDataPropagation = true
            };
            _previewWire = new Wire(previewWireModel, _wirePathingAlgorithum);
            diagram.AddWire(_previewWire);
        }

        private void WireTerminalsToWiringTerminal(Diagram diagram, Terminal terminal)
        {
            diagram.RemoveWire(_previewWire);
            TryWireTwoTerminalsOnDiagram(diagram, _wiringTerminal, terminal, _transactor, true);
            diagram.UnhighlightTerminals();
            diagram.UnselectTerminals();
            _previewWire = null;
            _previewTerminal = null;
            _wiringTerminal = null;
        }

        private void WireToTerminal(Diagram diagram, Terminal terminal)
        {
            if (_wiringTerminal == terminal)
            {
                CancelWireInsertion(diagram);
                return;
            }

            WireTerminalsToWiringTerminal(diagram, terminal);
        }
    }
}