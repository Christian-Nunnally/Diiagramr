using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace ColorOrgan5UnitTests.Features.Steps
{
    [Binding]
    public class WireSteps
    {
        private InputTerminal _inputTerminal;
        private OutputTerminal _outputTerminal;
        private Wire _wire;
        private WireViewModel _wireVm;

        [Given(@"I have a connected wire")]
        public void GivenIHaveAConnectedWire()
        {
            _inputTerminal = new InputTerminal("test", typeof(int), null);
            _outputTerminal = new OutputTerminal("test", typeof(int));
            _wire = new Wire(_outputTerminal, _inputTerminal);
            _wireVm = new WireViewModel(_wire);
        }

        [When(@"the input terminal is set to null")]
        public void WhenTheInputTerminalIsSetToNull()
        {
            _wire.SourceTerminal = null;
        }

        [When(@"the output terminal is set to null")]
        public void WhenTheOutputTerminalIsSetToNull()
        {
            _wire.SinkTerminal = null;
        }

        [Then(@"the output terminal should be disconnected")]
        public void ThenTheOutputTerminalShouldBeDisconnected()
        {
            Assert.IsNull(_outputTerminal.ConnectedWire);
        }

        [Then(@"the input terminal should be disconnected")]
        public void ThenTheInputTerminalShouldBeDisconnected()
        {
            Assert.IsNull(_inputTerminal.ConnectedWire);
        }
    }
}
