using DiiagramrAPI.Editor.Diagrams;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiiagramrUnitTests.Editor.Diagrams
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var node = new TestNode();
            node.CreateTerminals();
        }

        private class TestNode : Node
        {
            [InputTerminal(nameof(Input), DiiagramrModel.Direction.North)]
            public void Input(int number)
            {
            }
        }
    }
}