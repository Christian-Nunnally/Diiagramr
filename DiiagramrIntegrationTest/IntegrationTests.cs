using System;
using System.Collections.Generic;
using System.Linq;
using Diiagramr.PluginNodeApi;
using Diiagramr.Service;
using Diiagramr.View.CustomControls;
using Diiagramr.ViewModel;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Windows;
using Diiagramr.View;
using DiiagramrIntegrationTest.IntegrationHelpers;
using System.Windows.Forms;

namespace DiiagramrIntegrationTest
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void LoadSave()
        {
            var shell = TestSetup.SetupShellViewModel();
            var projExplorer = shell.ProjectExplorerViewModel;
            var projManager = projExplorer.ProjectManager;
            var diagramWellViewModel = shell.DiagramWellViewModel;
            var nodeSelectorViewModel = diagramWellViewModel.NodeSelectorViewModel;
            var testNode = nodeSelectorViewModel.AvailableNodeViewModels.First();

            shell.CreateProject();
            Assert.IsNotNull(projManager.CurrentProject);

            projManager.CreateDiagram();
            Assert.AreEqual(1, projManager.CurrentDiagrams.Count);

            // open diagram
            var diagramViewModel = shell.OpenDiagram();

            // add nodes
            var node1 = shell.PlaceNode(testNode, 5, 6);
            var node2 = shell.PlaceNode(testNode);

            // wire nodes
            var outTerm = node1.OutputTerminalViewModels.First();
            var inTerm = node2.InputTerminalViewModels.First();
            var wireViewModel = shell.WireTerminals(outTerm, inTerm);

            // set output
            node1.InputTerminalViewModels.First().Data = 4;
            outTerm.Data = 5;
            Assert.AreEqual(((TestNode) node1).OutputTerminal.Data, ((TestNode)node2).InputTerminal.Data);

            // move node2
            node2.X = 16;
            node2.Y = 17;
    
            var inTermNode2 = node2.InputTerminalViewModels.First().TerminalModel;
            var outTermNode1 = node1.OutputTerminalViewModels.First().TerminalModel;
            Assert.AreEqual(inTermNode2.X, node2.X);
            Assert.AreEqual(inTermNode2.Y, node2.Y);
            Assert.AreEqual(inTermNode2.NodeX + inTermNode2.OffsetX, inTermNode2.X);
            Assert.AreEqual(inTermNode2.NodeY + inTermNode2.OffsetY, inTermNode2.Y);
            Assert.AreEqual(inTermNode2.X + DiagramConstants.NodeBorderWidth, wireViewModel.X1);
            Assert.AreEqual(inTermNode2.Y + DiagramConstants.NodeBorderWidth, wireViewModel.Y1);
            Assert.AreEqual(outTermNode1.X + DiagramConstants.NodeBorderWidth, wireViewModel.X2);
            Assert.AreEqual(outTermNode1.Y + DiagramConstants.NodeBorderWidth, wireViewModel.Y2);

            // save
            projManager.SaveProject();
            projManager.CloseProject();
            projManager.LoadProject();

            // open diagram
            diagramViewModel = shell.OpenDiagram();
            Assert.AreEqual(2, diagramViewModel.NodeViewModels.Count);
            node1 = diagramViewModel.NodeViewModels[0];
            node2 = diagramViewModel.NodeViewModels[1];

            // check wire nodes
            outTerm = node1.OutputTerminalViewModels.First();
            inTerm = node2.InputTerminalViewModels.First();
            Assert.IsNotNull(outTerm.TerminalModel.ConnectedWire);
            Assert.IsNotNull(inTerm.TerminalModel.ConnectedWire);
            Assert.AreEqual(5, ((TestNode)node2).InputTerminal.Data);
            Assert.AreEqual(5, ((TestNode)node1).OutputTerminal.Data);
            Assert.AreEqual(6, ((TestNode)node2).OutputTerminal.Data);
            Assert.AreEqual(6, ((TestNode)node2).Value);
            Assert.AreEqual(((TestNode)node1).OutputTerminal.Data, ((TestNode)node2).InputTerminal.Data);
            

            // change data
            outTerm.Data = 6;
            Assert.AreEqual(((TestNode)node1).OutputTerminal.Data, ((TestNode)node2).InputTerminal.Data);
            Assert.AreEqual(((TestNode)node2).InputTerminal.Data + 1, ((TestNode)node2).OutputTerminal.Data);

            // change location
            node1.X = 11;
            node1.Y = 12;
            inTermNode2 = inTerm.TerminalModel;
            outTermNode1 = outTerm.TerminalModel;
            wireViewModel = diagramWellViewModel.ActiveItem.WireViewModels.First();
            Assert.AreEqual(inTermNode2.X, node2.X);
            Assert.AreEqual(inTermNode2.Y, node2.Y);
            Assert.AreEqual(inTermNode2.NodeX + inTermNode2.OffsetX, inTermNode2.X);
            Assert.AreEqual(inTermNode2.NodeY + inTermNode2.OffsetY, inTermNode2.Y);
            Assert.AreEqual(inTermNode2.X + DiagramConstants.NodeBorderWidth, wireViewModel.X1);
            Assert.AreEqual(inTermNode2.Y + DiagramConstants.NodeBorderWidth, wireViewModel.Y1);
            Assert.AreEqual(outTermNode1.X + DiagramConstants.NodeBorderWidth, wireViewModel.X2);
            Assert.AreEqual(outTermNode1.Y + DiagramConstants.NodeBorderWidth, wireViewModel.Y2);
        }

        [TestMethod]
        public void RunPauseStop()
        {
            var shell = TestSetup.SetupShellViewModel();
            var projExplorer = shell.ProjectExplorerViewModel;
            var projManager = projExplorer.ProjectManager;
            var diagramWellViewModel = shell.DiagramWellViewModel;
            var nodeSelectorViewModel = diagramWellViewModel.NodeSelectorViewModel;
            var testNode = nodeSelectorViewModel.AvailableNodeViewModels.First();

            shell.CreateProject();
            projManager.CreateDiagram();
            var diagramViewModel = shell.OpenDiagram();
            var node1 = shell.PlaceNode(testNode);
            var node2 = shell.PlaceNode(testNode);

            var outTerm = node1.OutputTerminalViewModels.First();
            var inTerm = node2.InputTerminalViewModels.First();
            shell.WireTerminals(outTerm, inTerm);


            var controlViewModel = shell.DiagramWellViewModel.ActiveItem.DiagramControlViewModel;

            // play 
            outTerm.Data = 5;
            Assert.AreEqual(outTerm.Data, inTerm.Data);
            Assert.AreEqual((int) inTerm.Data + 1, node2.OutputTerminalViewModels.First().Data);

            // pause
            controlViewModel.Pause();
            outTerm.Data = 6;
            Assert.AreEqual(5, inTerm.Data);
            Assert.AreEqual(6, node2.OutputTerminalViewModels.First().Data);

            // play after pause
            controlViewModel.Play();
            Assert.AreEqual(6, inTerm.Data);
            Assert.AreEqual(7, node2.OutputTerminalViewModels.First().Data);

            // stop
            controlViewModel.Stop();
            Assert.AreEqual(6, outTerm.Data);
            Assert.IsNull(inTerm.Data);

            // play after stop
            controlViewModel.Play();
            Assert.AreEqual(6, inTerm.Data);
            Assert.AreEqual(7, node2.OutputTerminalViewModels.First().Data);
        }
    }
}