﻿using System;
using Diiagramr.Model;
using Diiagramr.PluginNodeApi;
using Diiagramr.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DiiagramrUnitTests.PluginNodeApiTests
{
    [TestClass]
    public class TerminalTest
    {
        private TerminalModel _terminalModel;
        private TerminalViewModel _terminalViewModel;
        private Terminal<int> _terminal;


        [TestInitialize]
        public void TestInitialize()
        {
            _terminalModel = new TerminalModel("", typeof(int), Direction.None, TerminalKind.Input, 0);
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
    }
}
