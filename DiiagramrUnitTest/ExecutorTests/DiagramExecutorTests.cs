using Diiagramr.Executor;
using Diiagramr.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Drawing;

namespace ColorOrgan5UnitTests.ExecutorTests
{
    [TestClass]
    public class DiagramExecutorTests
    {
        private readonly DiagramExecutor _executor = new DiagramExecutor();

        [TestMethod]
        public void TestExecuteNoWire()
        {
            var outputTerminal = new OutputTerminal("test", typeof(int));
            _executor.Execute(outputTerminal, null);
        }

        [TestMethod]
        public void TestExecuteAcrossSingleWire()
        {
            var outputTerminal = new OutputTerminal("test", typeof(int));
            var inputTerminal = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var testNode = new DiagramExecutorTestDiagramNode(inputTerminal, new OutputTerminal("test", typeof(int)));
            var data = new Point(10, 10);

            new Wire(outputTerminal, inputTerminal);

            Assert.AreNotEqual(data, testNode.DataReceived);

            _executor.Execute(outputTerminal, data);

            Assert.AreEqual(data, testNode.DataReceived);
        }

        [TestMethod]
        public void TestExecuteAcrossTwoWires()
        {
            var output1 = new OutputTerminal("test", typeof(int));
            var input1 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var output2 = new OutputTerminal("test", typeof(int));
            var input2 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var output3 = new OutputTerminal("test", typeof(int));
            var testNode1 = new DiagramExecutorTestDiagramNode(input1, output2);
            var testNode2 = new DiagramExecutorTestDiagramNode(input2, output3);
            var data = new Point(10, 10);

            new Wire(output1, input1);
            new Wire(output2, input2);

            Assert.AreNotEqual(data, testNode2.DataReceived);

            _executor.Execute(output1, data);

            Assert.AreEqual(data, testNode2.DataReceived);
        }

        [TestMethod]
        public void TestExecuteBranchesToMultipleOutputs()
        {
            var output1 = new OutputTerminal("test", typeof(int));
            var input1 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var output21 = new OutputTerminal("test", typeof(int));
            var input21 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var output22 = new OutputTerminal("test", typeof(int));
            var input22 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var testNode1 = new DiagramExecutorTestDiagramNode(input1, output21);
            testNode1.AddOutputTerminal(output22);
            var testNode21 = new DiagramExecutorTestDiagramNode(input21, new OutputTerminal("test", typeof(int)));
            var testNode22 = new DiagramExecutorTestDiagramNode(input22, new OutputTerminal("test", typeof(int)));
            var data = new Point(10, 10);

            new Wire(output1, input1);
            new Wire(output21, input21);
            new Wire(output22, input22);

            Assert.AreNotEqual(data, testNode21.DataReceived);
            Assert.AreNotEqual(data, testNode22.DataReceived);

            _executor.Execute(output1, data);

            Assert.AreEqual(data, testNode21.DataReceived);
            Assert.AreEqual(data, testNode22.DataReceived);
        }

        [TestMethod]
        public void TestExecute_GoingInToDiagramNode()
        {
            var output1 = new OutputTerminal("test", typeof(int));
            var input1 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var output21 = new OutputTerminal("test", typeof(int));
            var input21 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var output22 = new OutputTerminal("test", typeof(int));
            var input22 = new InputTerminal("test", typeof(int), null); // TODO: Fix broken test
            var testNode1 = new DiagramExecutorTestDiagramNode(input1, output21);
            testNode1.AddOutputTerminal(output22);
            var testNode21 = new DiagramExecutorTestDiagramNode(input21, new OutputTerminal("test", typeof(int)));
            var testNode22 = new DiagramExecutorTestDiagramNode(input22, new OutputTerminal("test", typeof(int)));
            var data = new Point(10, 10);

            new Wire(output1, input1);
            new Wire(output21, input21);
            new Wire(output22, input22);

            Assert.AreNotEqual(data, testNode21.DataReceived);
            Assert.AreNotEqual(data, testNode22.DataReceived);

            _executor.Execute(output1, data);

            Assert.AreEqual(data, testNode21.DataReceived);
            Assert.AreEqual(data, testNode22.DataReceived);
        }
    }

    class DiagramExecutorTestDiagramNode : DiagramNode
    {
        public IList<InputTerminal> InputTerminals = new List<InputTerminal>();
        public IList<OutputTerminal> OutputTerminals = new List<OutputTerminal>();

        public object DataReceived;

        public DiagramExecutorTestDiagramNode(InputTerminal inputTerminal, OutputTerminal outputTerminal) : base("TestNodeViewModel")
        {
            InputTerminals.Add(inputTerminal);
            OutputTerminals.Add(outputTerminal);
            Terminals.Add(inputTerminal);
            Terminals.Add(outputTerminal);

            inputTerminal.SetInputTerminalDelegate("DelegateArg");
        }

        public void AddOutputTerminal(OutputTerminal outputTerminal)
        {
            OutputTerminals.Add(outputTerminal);
            Terminals.Add(outputTerminal);
        }

        private IDictionary<OutputTerminal, object> DelegateArg(object o)
        {
            DataReceived = o;
            IDictionary<OutputTerminal, object> outputs = new Dictionary<OutputTerminal, object>();
            foreach (var outputTerminal in OutputTerminals)
            {
                outputs.Add(outputTerminal, o);
            }
            return outputs;
        }
    }
}
