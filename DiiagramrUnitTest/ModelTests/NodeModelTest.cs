using Castle.Core.Internal;
using DiiagramrModel;
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
            Assert.AreEqual("name", _node.Name);
        }

        [TestMethod]
        public void TestAddTerminal_TerminalAddedToTerminals()
        {
            Assert.IsTrue(_node.Terminals.IsNullOrEmpty());

            _node.AddTerminal(_termMoq.Object);

            Assert.AreEqual(_termMoq.Object, _node.Terminals[0]);
        }

        [TestMethod]
        public void TestSetVariable_VariableIsPutInPersistedVariables()
        {
            _node.SetVariable("Key", "Value");
            Assert.AreEqual("Value", _node.PersistedVariables["Key"]);
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
    }
}