using DiiagramrAPI.Diagram;
using DiiagramrAPI.Diagram.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Reflection;

namespace DiiagramrUnitTests.PluginNodeApiTests
{
    [TestClass]
    public class PluginNodeTest
    {
        [TestMethod]
        public void TestInitializeWithNode_SetsNodeViewModel()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.InitializeWithNode(nodeMoq.Object);

            nodeMoq.VerifySet(n => n.NodeViewModel = testPluginNode);
        }

        [TestMethod]
        public void TestInitializeWithNode_DoesntSavePublicPropertyFromPluginNode()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode
            {
                Model = nodeMoq.Object,

                Width = 5
            };

            nodeMoq.Verify(n => n.SetVariable("Width", 0), Times.Never);
        }

        [TestMethod]
        public void TestInitializeWithNode_DoesntSavePublicPropertyWithoutPluginNodeSettingAttribute()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode
            {
                Model = nodeMoq.Object,

                PublicPropertyNonSetting = 5
            };

            nodeMoq.Verify(n => n.SetVariable("PublicPropertyNonSetting", 0), Times.Never);
        }

        [TestMethod]
        public void TestInitializePluginNodeSettings_InitializesNewSettingInNodeModelPersistedVariables()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode
            {
                Model = nodeMoq.Object
            };
            nodeMoq.SetupGet(n => n.NodeViewModel).Returns(testPluginNode);

            testPluginNode.InitializePluginNodeSettings();

            nodeMoq.Verify(n => n.InitializePersistedVariableToProperty(It.IsAny<PropertyInfo>()), Times.Once);
        }

        [TestMethod]
        public void TestInitializePluginNodeSettings_InitializesLoadedSettingOnPluginNode()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode
            {
                Model = nodeMoq.Object
            };
            nodeMoq.SetupGet(n => n.NodeViewModel).Returns(testPluginNode);

            testPluginNode.InitializePluginNodeSettings();

            nodeMoq.Verify(n => n.GetVariable(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void TestUnHighlightAllTerminals_SetsHighlightToFalseOnTerminalViewModels()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode
            {
                Model = nodeMoq.Object
            };
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalViewModelMoq = new Mock<Terminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.UnHighlightAllTerminals();

            terminalViewModelMoq.VerifySet(model => model.HighlightVisible = false);
        }

        [TestMethod]
        public void TestShowOutputTerminalLabelsOfType_TerminalTypeCompatible_SetsTitleVisibleToOnTerminalViewModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.HighlightOutputTerminalsOfType(typeof(int));

            terminalViewModelMoq.Verify(model => model.ShowHighlightIfCompatibleType(It.IsAny<Type>()));
        }

        [TestMethod]
        public void TestHighlightOutputTerminalsOfType_TerminalTypeNotCompatible_DoesNotSetHighlightVisibleTrueOnTerminalViewModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.HighlightOutputTerminalsOfType(typeof(string));

            terminalViewModelMoq.VerifySet(model => model.HighlightVisible = true, Times.Never);
        }

        [TestMethod]
        public void TestHighlightOutputTerminalsOfType_TerminalTypeCompatibleButInput_DoesNotSetHighlightVisibleToOnTerminalViewModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalViewModelMoq = new Mock<InputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.HighlightOutputTerminalsOfType(typeof(string));

            terminalViewModelMoq.VerifySet(model => model.HighlightVisible = true, Times.Never);
        }

        [TestMethod]
        public void TestHighlightInputTerminalsOfType_TerminalTypeCompatible_SetsHighlightVisibleTrueOnTerminalViewModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalViewModelMoq = new Mock<InputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.HighlightInputTerminalsOfType(typeof(int));

            terminalViewModelMoq.Verify(model => model.ShowHighlightIfCompatibleType(It.IsAny<Type>()));
        }

        [TestMethod]
        public void TestHighlightInputTerminalsOfType_TerminalTypeNotCompatible_DoesNotSetHighlightVisibleTrueOnTerminalViewModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Input, 0);
            var terminalViewModelMoq = new Mock<InputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.HighlightInputTerminalsOfType(typeof(string));

            terminalViewModelMoq.VerifySet(model => model.HighlightVisible = true, Times.Never);
        }

        [TestMethod]
        public void TestHighlightInputTerminalsOfType_TerminalTypeCompatibleButOutput_DoesNotSetHighlightVisibleToTrueOnTerminalViewModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.HighlightInputTerminalsOfType(typeof(string));

            terminalViewModelMoq.VerifySet(model => model.HighlightVisible = true, Times.Never);
        }

        [TestMethod]
        public void TestDisconnectAllTerminals_InvokesDisconnectTerminalOnTerminalViewModels()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            testPluginNode.TerminalViewModels.Add(terminalViewModelMoq.Object);

            testPluginNode.DisconnectAllTerminals();

            terminalViewModelMoq.Verify(n => n.DisconnectTerminal());
        }

        [TestMethod]
        public void TestAddTerminalViewModel_TerminalViewModelAddedToTerminalViewModels()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            var nodeMoq = new Mock<NodeModel>("");
            testPluginNode.Model = nodeMoq.Object;

            testPluginNode.AddTerminalViewModel(terminalViewModelMoq.Object);

            Assert.AreEqual(terminalViewModelMoq.Object, testPluginNode.TerminalViewModels.First());
        }

        [TestMethod]
        public void TestAddTerminalViewModel_TerminalModelAddedToNodeModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            var nodeMoq = new Mock<NodeModel>("");
            testPluginNode.Model = nodeMoq.Object;

            testPluginNode.AddTerminalViewModel(terminalViewModelMoq.Object);

            nodeMoq.Verify(n => n.AddTerminal(terminalMoq.Object));
        }

        [TestMethod]
        public void TestRemoveTerminalViewModel_TerminalViewModelRemovedFromTerminalViewModels()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            var nodeMoq = new Mock<NodeModel>("");
            testPluginNode.Model = nodeMoq.Object;

            testPluginNode.AddTerminalViewModel(terminalViewModelMoq.Object);
            testPluginNode.RemoveTerminalViewModel(terminalViewModelMoq.Object);

            Assert.AreEqual(0, testPluginNode.TerminalViewModels.Count);
        }

        [TestMethod]
        public void TestRemoveTerminalViewModel_TerminalModelRemovedFromNodeModel()
        {
            var testPluginNode = new TestPluginNode();
            var terminalMoq = new Mock<TerminalModel>("", typeof(int), Direction.North, TerminalKind.Output, 0);
            var terminalViewModelMoq = new Mock<OutputTerminal>(terminalMoq.Object);
            terminalViewModelMoq.SetupGet(n => n.TerminalModel).Returns(terminalMoq.Object);
            var nodeMoq = new Mock<NodeModel>("");
            testPluginNode.Model = nodeMoq.Object;

            testPluginNode.AddTerminalViewModel(terminalViewModelMoq.Object);
            testPluginNode.RemoveTerminalViewModel(terminalViewModelMoq.Object);

            nodeMoq.Verify(n => n.RemoveTerminal(terminalMoq.Object));
        }
    }

    internal class TestPluginNode : Node
    {
        private int _publicProperty;
        private int _publicPropertyNonSetting;
        public override string Name => "Test Node";

        [NodeSetting]
        public int PublicProperty
        {
            get => _publicProperty;

            set
            {
                _publicProperty = value;
                OnPropertyChanged(nameof(PublicProperty));
            }
        }

        public int PublicPropertyNonSetting
        {
            get => _publicPropertyNonSetting;

            set
            {
                _publicPropertyNonSetting = value;
                OnPropertyChanged(nameof(PublicPropertyNonSetting));
            }
        }

        protected override void SetupNode(NodeSetup setup)
        {
            setup.NodeSize(40, 40);
        }
    }
}
