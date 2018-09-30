using DiiagramrAPI.Model;
using DiiagramrAPI.ViewModel.Diagram;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DiiagramrUnitTests.ViewModelTests.Diagram
{
    [TestClass]
    public class DiagramControlViewModelTest
    {
        private Mock<DiagramModel> _diagramMoq;
        private DiagramControlViewModel _diagramControlViewModel;

        [TestInitialize]
        public void SetupTests()
        {
            _diagramMoq = new Mock<DiagramModel>();
            _diagramMoq.Setup(m => m.Play());
            _diagramControlViewModel = new DiagramControlViewModel(_diagramMoq.Object);
        }

        [TestMethod]
        public void ConstructorTest_DefaultPlay()
        {
            _diagramMoq.Verify(m => m.Play(), Times.Once);
            Assert.IsTrue(_diagramControlViewModel.PlayChecked);
        }

        [TestMethod]
        public void PlayTest_ToggleButtonsSet()
        {
            _diagramControlViewModel.Play();
            Assert.IsTrue(_diagramControlViewModel.PlayChecked);
            Assert.IsFalse(_diagramControlViewModel.PauseChecked);
            Assert.IsFalse(_diagramControlViewModel.StopChecked);
        }

        [TestMethod]
        public void PlayTest_DiagramPlayCalled()
        {
            _diagramControlViewModel.Play();
            _diagramMoq.Verify(m => m.Play(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void PauseTest_ToggleButtonsSet()
        {
            _diagramMoq.Setup(m => m.Pause());
            _diagramControlViewModel.Pause();
            Assert.IsTrue(_diagramControlViewModel.PauseChecked);
            Assert.IsFalse(_diagramControlViewModel.PlayChecked);
            Assert.IsFalse(_diagramControlViewModel.StopChecked);
        }

        [TestMethod]
        public void PauseTest_DiagramPauseCalled()
        {
            _diagramMoq.Setup(m => m.Pause());
            _diagramControlViewModel.Pause();
            _diagramMoq.Verify(m => m.Pause(), Times.Once);
        }

        [TestMethod]
        public void StopTest_ToggleButtonsSet()
        {
            _diagramMoq.Setup(m => m.Stop());
            _diagramControlViewModel.Stop();
            Assert.IsFalse(_diagramControlViewModel.PlayChecked);
            Assert.IsFalse(_diagramControlViewModel.PauseChecked);
            Assert.IsTrue(_diagramControlViewModel.StopChecked);
        }

        [TestMethod]
        public void StopTest_DiagramStopCalled()
        {
            _diagramMoq.Setup(m => m.Stop());
            _diagramControlViewModel.Stop();
            _diagramMoq.Verify(m => m.Stop(), Times.Once);
        }
    }
}
