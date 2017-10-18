using System.ComponentModel;
using Castle.Core.Internal;
using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class NodeModelTest
    {
        private NodeModel _node;
        private Mock<TerminalModel> _termMoq;

        [TestInitialize]
        public void SetupTests()
        {
            _node = new NodeModel("name");
            _termMoq = new Mock<TerminalModel>();
        }

        [TestMethod]
        public void TestConstructor_TerminalsCollectionNotNull()
        {
            Assert.IsNotNull(_node.Terminals);
        }

        [TestMethod]
        public void TestConstructor_NodeTypeSetToName()
        {
            Assert.AreEqual("name", _node.NodeFullName);
        }

        [TestMethod]
        public void TestAddTerminal_TerminalAddedToTerminals()
        {
            Assert.IsTrue(_node.Terminals.IsNullOrEmpty());

            _node.AddTerminal(_termMoq.Object);

            Assert.AreEqual(_termMoq.Object, _node.Terminals[0]);
        }

        [TestMethod]
        public void TestAddTerminal_TerminalNotfiedOfNodePropertyChanges()
        {
            _node.AddTerminal(_termMoq.Object);
            _node.X++;

            _termMoq.Verify(m => m.NodePropertyChanged(It.IsAny<object>(), It.IsAny<PropertyChangedEventArgs>()));
        }

        [TestMethod]
        public void TestSetTerminalsPropertyChanged_TerminalNotfiedOfNodePropertyChanges()
        {
            _node.Terminals.Add(_termMoq.Object);
            _node.SetTerminalsPropertyChanged();
            _node.X++;

            _termMoq.Verify(m => m.NodePropertyChanged(It.IsAny<object>(), It.IsAny<PropertyChangedEventArgs>()));
        }

        [TestMethod]
        public void TestSetVariable_VariableIsPutInPersistedVariables()
        {
            _node.SetVariable("Key", "Value");
            Assert.AreEqual("Value", _node.PersistedVariables["Key"]);
        }

        [TestMethod]
        public void TestSetVariable_SemanticsChangedInvoked()
        {
            var semanticsChanged = false;
            _node.SemanticsChanged += () => semanticsChanged = true;

            _node.SetVariable("Key", "Value");

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestGetVariable_VariableNeverSet_ReturnsNull()
        {
            Assert.IsNull(_node.GetVariable("Key"));
        }

        [TestMethod]
        public void TestGetVariable_VariableSet_ReturnsValue()
        {
            _node.SetVariable("Key", "Value");
            Assert.AreEqual("Value", _node.GetVariable("Key"));
        }

        [TestMethod]
        public void TestSemanticsChanged_TerminalAdded_SematicsChangedInvoked()
        {
            var semanticsChanged = false;
            _node.SemanticsChanged += () => semanticsChanged = true;
            _node.AddTerminal(_termMoq.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestSemanticsChanged_TerminalSemanticsChanged_SematicsChangedInvoked()
        {
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var semanticsChanged = false;
            _node.SemanticsChanged += () => semanticsChanged = true;
            _node.AddTerminal(terminalMoq.Object);

            terminalMoq.Raise(n => n.SemanticsChanged += null);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestEnableTerminals_CallsEnableWire()
        {
            _node.AddTerminal(_termMoq.Object);
            _node.EnableTerminals();
            _termMoq.Verify(m => m.EnableWire(), Times.Once);
        }

        [TestMethod]
        public void TestResetTerminals_CallsResetWire()
        {
            _node.AddTerminal(_termMoq.Object);
            _node.ResetTerminals();
            _termMoq.Verify(m => m.ResetWire(), Times.Once);
        }

        [TestMethod]
        public void TestDisableTerminals_CallsDisableWire()
        {
            _node.AddTerminal(_termMoq.Object);
            _node.DisableTerminals();
            _termMoq.Verify(m => m.DisableWire(), Times.Once);
        }

        [TestMethod]
        public void TestSetNodeViewModel_InitializePluginNodeSettingsInvokedOnViewModel()
        {
            var nodeViewModelMoq = new Mock<PluginNode>();
            _node.NodeViewModel = nodeViewModelMoq.Object;
            nodeViewModelMoq.Verify(n => n.InitializePluginNodeSettings());
        }
    }
}
