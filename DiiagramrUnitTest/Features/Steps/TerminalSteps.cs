using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow;

namespace ColorOrgan5UnitTests.Features.Steps
{
    [Binding]
    public class TerminalFeatureSteps
    {
        private InputTerminalViewModel _inputTerminal;
        private OutputTerminalViewModel _outputTerminal;

        [Given(@"I have an output terminal")]
        public void GivenIHaveAnOutputTerminal()
        {
            _outputTerminal = new OutputTerminalViewModel(new OutputTerminal("test", typeof(int)));
        }

        [Given(@"I have an input terminal")]
        public void GivenIHaveAnInputTerminal()
        {
            _inputTerminal = new InputTerminalViewModel(new InputTerminal("test", typeof(int), null));
        }

        [When(@"I drop the output terminal on the input terminal")]
        public void WhenIDropTheOutputTerminalOnTheInputTerminal()
        {
            _inputTerminal.DropObject(_outputTerminal.Terminal);
        }

        [When(@"I drop the input terminal on the output terminal")]
        public void WhenIDropTheInputTerminalOnTheOutputTerminal()
        {
            _outputTerminal.DropObject(_inputTerminal.Terminal);
        }

        [Then(@"The output terminal should be wired to the input terminal")]
        public void ThenTheOutputTerminalShouldBeWiredToTheInputTerminal()
        {
            Assert.IsNotNull(_inputTerminal.Terminal.ConnectedWire);
            Assert.IsNotNull(_outputTerminal.Terminal.ConnectedWire);
            Assert.AreEqual(_inputTerminal.Terminal.ConnectedWire, _outputTerminal.Terminal.ConnectedWire);
            Assert.AreEqual(_inputTerminal.Terminal.ConnectedWire.SinkTerminal, _inputTerminal.Terminal);
            Assert.AreEqual(_outputTerminal.Terminal.ConnectedWire.SourceTerminal, _outputTerminal.Terminal);
        }
    }
}