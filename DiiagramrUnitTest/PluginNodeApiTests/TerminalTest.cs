using DiiagramrAPI.Model;
using DiiagramrAPI.PluginNodeApi;
using DiiagramrAPI.ViewModel.ProjectScreen.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DiiagramrUnitTests.PluginNodeApiTests
{
    [TestClass]
    public class TerminalTest
    {
        private Terminal<int> _terminal;
        private TerminalModel _terminalModel;
        private TerminalViewModel _terminalViewModel;

        [TestInitialize]
        public void TestInitialize()
        {
            _terminalModel = new TerminalModel("", typeof(int), Direction.North, TerminalKind.Input, 0);
            _terminalViewModel = new TerminalViewModel(_terminalModel);
            _terminal = new Terminal<int>(_terminalViewModel);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Terminal should not allow null underlyingTerminal")]
        public void TestConstructor_NullArguments_ThrowsException()
        {
            new Terminal<int>(null);
        }

        [TestMethod]
        public void TestConstructor_UnderlyingTerminalDataChanged_ChangesData()
        {
            _terminalViewModel.Data = 5;
            Assert.AreEqual(_terminalViewModel.Data, _terminal.Data);
        }

        [TestMethod]
        public void TestSetData_DataIsTheSame_DataChangedNotInvoked()
        {
            var dataChangedCalled = false;
            _terminal.DataChanged += i => dataChangedCalled = true;
            dataChangedCalled = false;
            _terminal.Data += 0;
            Assert.IsFalse(dataChangedCalled);
        }

        [TestMethod]
        public void TestSetData_DataIsChanged_DataChangedInvoked()
        {
            var dataChangedCalled = false;
            _terminal.DataChanged += i => dataChangedCalled = true;
            _terminal.Data++;
            Assert.IsTrue(dataChangedCalled);
        }

        [TestMethod]
        public void TestSetData_DataIsChanged_UnderlyingTerminalViewModelDataSet()
        {
            _terminal.Data++;
            Assert.AreEqual(_terminalViewModel.Data, _terminal.Data);
        }

        [TestMethod]
        public void TestSetData_UnderlyingTerminalViewModelDataNullIntSet()
        {
            _terminalViewModel.Data = null;
            var terminal = new Terminal<int>(_terminalViewModel);
            Assert.AreEqual(terminal.Data, 0);
        }

        [TestMethod]
        public void TestSetData_UnderlyingTerminalViewModelDataNullStringSet()
        {
            _terminalViewModel.Data = null;
            var terminal = new Terminal<string>(_terminalViewModel);
            Assert.IsNull(terminal.Data);
        }

        [TestMethod]
        public void TestChangeTerminalData_SetsTerminalData()
        {
            var terminal = new Terminal<string>(_terminalViewModel);
            terminal.ChangeTerminalData("Hello");
            Assert.AreEqual(terminal.Data, "Hello");
        }
    }
}