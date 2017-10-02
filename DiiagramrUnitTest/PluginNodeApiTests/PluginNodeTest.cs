using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.PluginNodeApiTests
{
    [TestClass]
    public class PluginNodeTest
    {
        [TestMethod]
        public void TestSaveNodeVariables_FindsImplementingPublicProperty()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.InitializeWithNode(nodeMoq.Object);

            testPluginNode.PublicProperty = 5;

            nodeMoq.Verify(n => n.SetVariable("PublicProperty", 5));
        }

        [TestMethod]
        public void TestSaveNodeVariables_DoesntSavePublicPropertyFromPluginNode()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.Width = 5;

            nodeMoq.Verify(n => n.SetVariable("Width", 0), Times.Never);
        }

        [TestMethod]
        public void TestSaveNodeVariables_DoesntSavePublicPropertyWithoutPluginNodeSettingAttribute()
        {
            var nodeMoq = new Mock<NodeModel>("");
            var testPluginNode = new TestPluginNode();
            testPluginNode.NodeModel = nodeMoq.Object;

            testPluginNode.PublicPropertyNonSetting = 5;

            nodeMoq.Verify(n => n.SetVariable("PublicPropertyNonSetting", 0), Times.Never);
        }
    }

    internal class TestPluginNode : PluginNode
    {
        private int _publicProperty;
        private int _publicPropertyNonSetting;
        public override string Name => "Test Node";

        [PluginNodeSetting]
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

        public override void SetupNode(NodeSetup setup)
        {
        }
    }
}
