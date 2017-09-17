using System;
using System.ComponentModel;
using System.Linq;
using Castle.Core.Internal;
using Diiagramr.Model;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ModelTests
{
    [TestClass]
    public class NodeModelTest
    {
        [TestMethod]
        public void TestConstructor_TerminalsCollectionNotNull()
        {
            var node = new NodeModel("");
            Assert.IsNotNull(node.Terminals);
        }

        [TestMethod]
        public void TestConstructor_NodeTypeSetToName()
        {
            var node = new NodeModel("name");
            Assert.AreEqual("name", node.NodeFullName);
        }

        [TestMethod]
        public void TestAddTerminal_TerminalAddedToTerminals()
        {
            var node = new NodeModel("name");
            var terminalMoq = new Mock<TerminalModel>();
            Assert.IsTrue(node.Terminals.IsNullOrEmpty());

            node.AddTerminal(terminalMoq.Object);

            Assert.AreEqual(terminalMoq.Object, node.Terminals[0]);
        }

        [TestMethod]
        public void TestAddTerminal_TerminalNotfiedOfNodePropertyChanges()
        {
            var node = new NodeModel("name");
            var terminalMoq = new Mock<TerminalModel>();

            node.AddTerminal(terminalMoq.Object);
            node.X++;

            terminalMoq.Verify(m => m.NodePropertyChanged(It.IsAny<object>(), It.IsAny<PropertyChangedEventArgs>()));
        }

        [TestMethod]
        public void TestSetTerminalsPropertyChanged_TerminalNotfiedOfNodePropertyChanges()
        {
            var node = new NodeModel("name");
            var terminalMoq = new Mock<TerminalModel>();

            node.Terminals.Add(terminalMoq.Object);
            node.SetTerminalsPropertyChanged();
            node.X++;

            terminalMoq.Verify(m => m.NodePropertyChanged(It.IsAny<object>(), It.IsAny<PropertyChangedEventArgs>()));
        }

        [TestMethod]
        public void TestSetVariable_VariableIsPutInPersistedVariables()
        {
            var node = new NodeModel("name");
            node.SetVariable("Key", "Value");
            Assert.AreEqual("Value", node.PersistedVariables["Key"]);
        }

        [TestMethod]
        public void TestGetVariable_VariableNeverSet_ReturnsNull()
        {
            var node = new NodeModel("name");
            Assert.IsNull(node.GetVariable("Key"));
        }

        [TestMethod]
        public void TestGetVariable_VariableSet_ReturnsValue()
        {
            var node = new NodeModel("name");
            node.SetVariable("Key", "Value");
            Assert.AreEqual("Value", node.GetVariable("Key"));
        }

        [TestMethod]
        public void TestPreSave_HasNodeViewModel_CallsSaveNodeVariablesOnNodeViewModel()
        {
            var node = new NodeModel("name");
            var nodeViewModelMoq = new Mock<AbstractNodeViewModel>();
            node.NodeViewModel = nodeViewModelMoq.Object;

            node.PreSave();

            nodeViewModelMoq.Verify(d => d.SaveNodeVariables());
        }

        [TestMethod]
        public void TestSemanticsChanged_TerminalAdded_SematicsChangedInvoked()
        {
            var node = new NodeModel("name");
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var semanticsChanged = false;
            node.SemanticsChanged += () => semanticsChanged = true;
            node.AddTerminal(terminalMoq.Object);

            Assert.IsTrue(semanticsChanged);
        }

        [TestMethod]
        public void TestSemanticsChanged_TerminalSemanticsChanged_SematicsChangedInvoked()
        {
            var node = new NodeModel("name");
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var semanticsChanged = false;
            node.SemanticsChanged += () => semanticsChanged = true;
            node.AddTerminal(terminalMoq.Object);

            terminalMoq.Raise(n => n.SemanticsChanged += null);

            Assert.IsTrue(semanticsChanged);
        }
    }
}
